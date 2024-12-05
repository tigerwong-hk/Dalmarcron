import { Logger } from "@aws-lambda-powertools/logger";
import { Transform } from "@aws-lambda-powertools/parameters";
import { getParameter, getParametersByName } from "@aws-lambda-powertools/parameters/ssm";
import type { SSMGetParametersByNameOptions } from "@aws-lambda-powertools/parameters/ssm/types";
import { v4 as uuidv4 } from "uuid";
import { createOauth2ApiService } from "./oauth2ApiService.mts";
// import { createRedisCacheService } from "./redisCacheService.mts";
import { IApiService } from "./iApiService.mts";
// import { ICacheService } from "./iCacheService.mts";
import { IJobService } from "./iJobService.mts";

const apiMethod: string = process.env["API_METHOD"] ?? "GET";

const apiIdempotencyKey: string = process.env["API_IDEMPOTENCY_KEY"] ?? "";

console.log("apiMethod: ", apiMethod);
console.log("apiIdempotencyKey: ", apiIdempotencyKey);

const ssmParametersPathPrefix: string = process.env["SSM_PARAMETERS_PATH_PREFIX"] ?? "";
if (ssmParametersPathPrefix.trim().length < 1) {
  throw new Error("SSM parameters path prefix missing");
}

const apiUrl: string =
  (await getParameter(`${ssmParametersPathPrefix}/api/url`, { decrypt: true })) ?? "";
if (apiUrl.trim().length < 1) {
  throw new Error("API URL missing");
}

const apiHeadersOptions: Record<string, SSMGetParametersByNameOptions> = {
  [`${ssmParametersPathPrefix}/api/headers`]: { decrypt: true, transform: Transform.JSON },
};

const { _errors: apiHeadersErrors, ...apiHeadersParameters } =
  await getParametersByName<HeadersInit>(apiHeadersOptions, {
    throwOnError: false,
  });

const apiHeaders: HeadersInit = apiHeadersErrors?.length
  ? {}
  : (apiHeadersParameters[`${ssmParametersPathPrefix}/api/headers`] ?? {});

console.log("apiHeaders: ", JSON.stringify(apiHeaders));

const apiJsonBodyOptions: Record<string, SSMGetParametersByNameOptions> = {
  [`${ssmParametersPathPrefix}/api/jsonBody`]: { decrypt: true, transform: Transform.JSON },
};

const { _errors: apiJsonBodyErrors, ...apiJsonBodyParameters } = await getParametersByName<
  Record<string, unknown>
>(apiJsonBodyOptions, {
  throwOnError: false,
});

const apiJsonBody: Record<string, unknown> | null = apiJsonBodyErrors?.length
  ? null
  : (apiJsonBodyParameters[`${ssmParametersPathPrefix}/api/jsonBody`] ?? null);

console.log("apiJsonBody: ", JSON.stringify(apiJsonBody));

const execute = (logger: Logger) => async (): Promise<void> => {
  /*
  const cacheService: ICacheService = createRedisCacheService(logger);
  const cacheKey = "test";
  const cacheValue = "one";

  const keyExistsBeforeSet: boolean = await cacheService.keyExists(cacheKey);
  logger.info("keyExists", keyExistsBeforeSet.toString());

  const setResult: string | null = await cacheService.setString(cacheKey, cacheValue, 60);
  logger.info("setResult", setResult ? setResult : "");

  const getValue: string | null = await cacheService.getString(cacheKey);
  logger.info("getValue", getValue ? getValue : "");

  const keyRemoved: boolean = await cacheService.removeKey(cacheKey);
  logger.info("keyRemoved", keyRemoved.toString());

  const keyExistsAfterDelete: boolean = await cacheService.keyExists(cacheKey);
  logger.info("keyExists", keyExistsAfterDelete.toString());
  */

  logger.info("apiUrl", apiUrl);

  if (apiMethod.toUpperCase() === "POST" && apiIdempotencyKey.trim().length > 0 && apiJsonBody) {
    apiJsonBody[apiIdempotencyKey] = uuidv4();
  }

  console.log("apiJsonBody (POST): ", JSON.stringify(apiJsonBody));

  const apiService: IApiService = createOauth2ApiService(logger);
  const requestOptions: RequestInit = {
    method: apiMethod,
    headers: apiHeaders,
    body:
      (apiMethod.toUpperCase() === "POST" || apiMethod.toUpperCase() === "PUT") && apiJsonBody
        ? JSON.stringify(apiJsonBody)
        : null,
  };

  try {
    const response: Response = await apiService.fetch(apiUrl, requestOptions);
    if (!response.ok) {
      throw new Error(`Response status: ${response.status.toString()} ${response.statusText}`);
    }

    logger.info("Fetch API response ok", response.ok.toString());
    logger.info("Fetch API response status", response.status.toString());
    logger.info("Fetch API response status text", response.statusText);
    logger.info("Fetch API response type", response.type);
    logger.info("Fetch API response url", response.url);
    logger.info("Fetch API response redirected", response.redirected.toString());

    const json: object = (await response.json()) as object;
    logger.info("Fetch API response json", JSON.stringify(json));
  } catch (err) {
    logger.error("Fetch exception", err instanceof Error ? err : JSON.stringify(err));
    throw err;
  }
};

export const createCronJobService = (logger: Logger): IJobService => {
  return {
    execute: execute(logger),
  };
};
