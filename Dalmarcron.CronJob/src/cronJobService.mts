import { Logger } from "@aws-lambda-powertools/logger";
import { Transform } from "@aws-lambda-powertools/parameters";
import { getParameter, getParametersByName } from "@aws-lambda-powertools/parameters/ssm";
import type { SSMGetParametersByNameOptions } from "@aws-lambda-powertools/parameters/ssm/types";
import { v4 as uuidv4 } from "uuid";
import { createFetchApiService } from "./fetchApiService.mts";
import { createOauth2ApiService } from "./oauth2ApiService.mts";
// import { createRedisCacheService } from "./redisCacheService.mts";
import { IApiService } from "./iApiService.mts";
// import { ICacheService } from "./iCacheService.mts";
import { IJobService } from "./iJobService.mts";

const apiIdempotencyKey: string = process.env["API_IDEMPOTENCY_KEY"] ?? "";

const apiMethod: string = process.env["API_METHOD"] ?? "GET";

const apiType: string = process.env["API_TYPE"] ?? "";
if (apiType.trim().length < 1) {
  throw new Error("API type missing");
}

console.log("apiIdempotencyKey: ", apiIdempotencyKey);
console.log("apiMethod: ", apiMethod);
console.log("apiType: ", apiType);

switch (apiMethod) {
  case "GET":
    break;
  case "POST":
    break;
  case "PUT":
    break;
  default:
    throw new Error(`API method invalid: ${apiMethod}`);
}

switch (apiType) {
  case "FETCH":
    break;
  case "OAUTH2":
    break;
  default:
    throw new Error(`API type invalid: ${apiType}`);
}

const ssmParametersPathPrefix: string = process.env["SSM_PARAMETERS_PATH_PREFIX"] ?? "";
if (ssmParametersPathPrefix.trim().length < 1) {
  throw new Error("SSM parameters path prefix missing");
}

const apiUrl: string =
  (await getParameter(`${ssmParametersPathPrefix}/ApiUrl`, { decrypt: true })) ?? "";
if (apiUrl.trim().length < 1) {
  throw new Error("API URL missing");
}

const apiHeadersOptions: Record<string, SSMGetParametersByNameOptions> = {
  [`${ssmParametersPathPrefix}/ApiHeaders`]: { decrypt: true, transform: Transform.JSON },
};

const { _errors: apiHeadersErrors, ...apiHeadersParameters } =
  await getParametersByName<HeadersInit>(apiHeadersOptions, {
    throwOnError: false,
  });

const apiHeaders: HeadersInit = apiHeadersErrors?.length
  ? {}
  : (apiHeadersParameters[`${ssmParametersPathPrefix}/ApiHeaders`] ?? {});

console.log("apiHeaders: ", JSON.stringify(apiHeaders));

const apiJsonBodyOptions: Record<string, SSMGetParametersByNameOptions> = {
  [`${ssmParametersPathPrefix}/ApiJsonBody`]: { decrypt: true, transform: Transform.JSON },
};

const { _errors: apiJsonBodyErrors, ...apiJsonBodyParameters } = await getParametersByName<
  Record<string, unknown>
>(apiJsonBodyOptions, {
  throwOnError: false,
});

const apiJsonBody: Record<string, unknown> | null = apiJsonBodyErrors?.length
  ? null
  : (apiJsonBodyParameters[`${ssmParametersPathPrefix}/ApiJsonBody`] ?? null);

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

  const apiService: IApiService =
    apiType === "OAUTH2" ? createOauth2ApiService(logger) : createFetchApiService(logger);
  const requestOptions: RequestInit = {
    method: apiMethod,
    headers: apiHeaders,
    body:
      (apiMethod.toUpperCase() === "POST" || apiMethod.toUpperCase() === "PUT") && apiJsonBody
        ? JSON.stringify(apiJsonBody)
        : null,
  };

  try {
    const response: Response = await apiService.send(apiUrl, requestOptions);
    if (!response.ok) {
      throw new Error(`Response status: ${response.status.toString()} ${response.statusText}`);
    }

    logger.info("API service send response ok", response.ok.toString());
    logger.info("API service send response status", response.status.toString());
    logger.info("API service send response status text", response.statusText);
    logger.info("API service send response type", response.type);
    logger.info("API service send response url", response.url);
    logger.info("API service send response redirected", response.redirected.toString());

    const json: object = (await response.json()) as object;
    logger.info("API service send response json", JSON.stringify(json));
  } catch (err) {
    logger.error("API service send exception", err instanceof Error ? err : JSON.stringify(err));
    throw err;
  }
};

export const createCronJobService = (logger: Logger): IJobService => {
  return {
    execute: execute(logger),
  };
};
