import type { Context, EventBridgeEvent, EventBridgeHandler } from "aws-lambda";
import { Logger } from "@aws-lambda-powertools/logger";
import { createRedisCacheService } from "./redisCacheService.mts";
import { ICacheService } from "./iCacheService.mts";

const logger: Logger = new Logger({ serviceName: "schedulerService" });

export const lambdaHandler: EventBridgeHandler<"Scheduled Event", null, void> = async (
  event: EventBridgeEvent<"Scheduled Event", null>,
  context: Context,
): Promise<void> => {
  logger.info("Event", JSON.stringify(event));
  logger.info("Context", JSON.stringify(context));

  const cacheService: ICacheService = createRedisCacheService(logger);
  const cacheKey = "test";
  const cacheValue = "one";

  const keyExistsBeforeSet: boolean = await cacheService.keyExists(cacheKey);
  logger.info("keyExists:", keyExistsBeforeSet.toString());

  const setResult: string | null = await cacheService.setString(cacheKey, cacheValue, 60);
  logger.info("setResult:", setResult ? setResult : "");

  const getValue: string | null = await cacheService.getString(cacheKey);
  logger.info("getValue:", getValue ? getValue : "");

  const keyRemoved: boolean = await cacheService.removeKey(cacheKey);
  logger.info("keyRemoved:", keyRemoved.toString());

  const keyExistsAfterDelete: boolean = await cacheService.keyExists(cacheKey);
  logger.info("keyExists:", keyExistsAfterDelete.toString());
};
