import { Logger } from "@aws-lambda-powertools/logger";
import { IApiService } from "./iApiService.mts";

const send =
  (logger: Logger) =>
  async (input: RequestInfo, init?: RequestInit): Promise<Response> => {
    try {
      const response: Response = await fetch(input, init);
      if (!response.ok) {
        throw new Error(`Response status: ${response.status.toString()} ${response.statusText}`);
      }

      logger.info("Fetch API response ok", response.ok.toString());
      logger.info("Fetch API response status", response.status.toString());
      logger.info("Fetch API response status text", response.statusText);
      logger.info("Fetch API response type", response.type);
      logger.info("Fetch API response url", response.url);
      logger.info("Fetch API response redirected", response.redirected.toString());

      return response;
    } catch (err) {
      logger.error("Fetch exception", err instanceof Error ? err : JSON.stringify(err));
      throw err;
    }
  };

export const createFetchApiService = (logger: Logger): IApiService => {
  return {
    send: send(logger),
  };
};
