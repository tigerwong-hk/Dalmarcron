import { Logger } from "@aws-lambda-powertools/logger";
import { createClient, RedisClientType, SocketClosedUnexpectedlyError } from "@redis/client";
import { ICacheService } from "./iCacheService.mts";

const redisUrl: string = process.env["REDIS_URL"] ?? "";
if (redisUrl.trim().length < 1) {
  throw new Error("Redis URL undefined");
}

let redisClient: RedisClientType | null;

async function createRedisClient(logger: Logger): Promise<RedisClientType> {
  const client: RedisClientType = createClient({
    url: redisUrl,
  });

  client.on("connect", () => {
    logger.info("Redis connection connected");
  });
  client.on("end", () => {
    logger.info("Redis connection ended");
  });
  client.on("error", (err: Error) => {
    logger.error("Redis connection error", err);
  });
  client.on("ready", () => {
    logger.info("Redis connection ready");
  });
  client.on("reconnecting", () => {
    logger.info("Redis connection reconnecting");
  });

  await client.connect();

  return client;
}

async function getRedisClient(logger: Logger): Promise<RedisClientType> {
  if (!redisClient) {
    redisClient = await createRedisClient(logger);
  }

  return redisClient;
}

const getString =
  (logger: Logger) =>
  async (cacheKey: string, numTries = 1): Promise<string | null> => {
    let value: string | null = null;

    if (cacheKey.trim().length < 1) {
      throw new Error("Empty cache key");
    }

    while (numTries-- > 0) {
      try {
        const client: RedisClientType = await getRedisClient(logger);
        value = await client.get(cacheKey);
        break;
      } catch (err) {
        if (err instanceof SocketClosedUnexpectedlyError) {
          redisClient = null;
          continue;
        }

        logger.error("Get exception", err instanceof Error ? err : JSON.stringify(err));
        throw err;
      }
    }

    return value;
  };

const keyExists =
  (logger: Logger) =>
  async (cacheKey: string, numTries = 1): Promise<boolean> => {
    let result = 0;

    if (cacheKey.trim().length < 1) {
      throw new Error("Empty cache key");
    }

    while (numTries-- > 0) {
      try {
        const client: RedisClientType = await getRedisClient(logger);
        result = await client.exists(cacheKey);
        break;
      } catch (err) {
        if (err instanceof SocketClosedUnexpectedlyError) {
          redisClient = null;
          continue;
        }

        logger.error("Exists exception", err instanceof Error ? err : JSON.stringify(err));
        throw err;
      }
    }

    return result === 1;
  };

const removeKey =
  (logger: Logger) =>
  async (cacheKey: string, numTries = 1): Promise<boolean> => {
    let result = 0;

    if (cacheKey.trim().length < 1) {
      throw new Error("Empty cache key");
    }

    while (numTries-- > 0) {
      try {
        const client: RedisClientType = await getRedisClient(logger);
        result = await client.del(cacheKey);
        break;
      } catch (err) {
        if (err instanceof SocketClosedUnexpectedlyError) {
          redisClient = null;
          continue;
        }

        logger.error("Remove exception", err instanceof Error ? err : JSON.stringify(err));
        throw err;
      }
    }

    return result === 1;
  };

const setString =
  (logger: Logger) =>
  async (
    cacheKey: string,
    value: string,
    expireSeconds = 0,
    numTries = 1,
  ): Promise<string | null> => {
    let result: string | null = null;

    if (cacheKey.trim().length < 1) {
      throw new Error("Empty cache key");
    }

    while (numTries-- > 0) {
      try {
        const client: RedisClientType = await getRedisClient(logger);
        result =
          expireSeconds > 0
            ? await client.set(cacheKey, value, {
                EX: expireSeconds,
              })
            : await client.set(cacheKey, value);
        logger.info("Set result", result ? result : "<null>");
        break;
      } catch (err) {
        if (err instanceof SocketClosedUnexpectedlyError) {
          redisClient = null;
          continue;
        }

        logger.error("Set exception", err instanceof Error ? err : JSON.stringify(err));
        throw err;
      }
    }

    return result;
  };

export const createRedisCacheService = (logger: Logger): ICacheService => {
  return {
    getString: getString(logger),
    keyExists: keyExists(logger),
    removeKey: removeKey(logger),
    setString: setString(logger),
  };
};
