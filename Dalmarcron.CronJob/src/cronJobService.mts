import { Logger } from "@aws-lambda-powertools/logger";
import { OAuth2Fetch } from "@badgateway/oauth2-client";
import { createOauth2ApiService } from "./oauth2ApiService.mts";
// import { createRedisCacheService } from "./redisCacheService.mts";
// import { ICacheService } from "./iCacheService.mts";
import { IJobService } from "./iJobService.mts";

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

  const apiUrl: string = process.env["API_URL"] ?? "";
  if (apiUrl.trim().length < 1) {
    throw new Error("API URL missing");
  }

  const apiService: OAuth2Fetch = createOauth2ApiService(logger);

  try {
    logger.info("API URL", apiUrl);
    logger.info("Access token", await apiService.getAccessToken());

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

    const json = await response.json();
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
