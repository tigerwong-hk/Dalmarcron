import { Logger } from "@aws-lambda-powertools/logger";
import { getParameter } from "@aws-lambda-powertools/parameters/ssm";
import { createOauth2ApiService } from "./oauth2ApiService.mts";
// import { createRedisCacheService } from "./redisCacheService.mts";
import { IApiService } from "./iApiService.mts";
// import { ICacheService } from "./iCacheService.mts";
import { IJobService } from "./iJobService.mts";

const ssmParametersPathPrefix: string = process.env["SSM_PARAMETERS_PATH_PREFIX"] ?? "";
if (ssmParametersPathPrefix.trim().length < 1) {
  throw new Error("SSM parameters path prefix missing");
}

const apiUrl: string =
  (await getParameter(`${ssmParametersPathPrefix}/api/url`, { decrypt: true })) ?? "";
if (apiUrl.trim().length < 1) {
  throw new Error("API URL missing");
}

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

  const apiService: IApiService = createOauth2ApiService(logger);

  try {
    const response: Response = await apiService.fetch(apiUrl);
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
