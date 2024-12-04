import { Logger } from "@aws-lambda-powertools/logger";
import { OAuth2Client, OAuth2Fetch } from "@badgateway/oauth2-client";
import { IApiService } from "./iApiService.mts";

const oauth2BaseUri: string = process.env["OAUTH2_BASE_URI"] ?? "";
if (oauth2BaseUri.trim().length < 1) {
  throw new Error("Oauth2 base URI missing");
}

const oauth2ClientId: string = process.env["OAUTH2_CLIENT_ID"] ?? "";
if (oauth2ClientId.trim().length < 1) {
  throw new Error("Oauth2 client ID missing");
}

const oauth2ClientScope: string = process.env["OAUTH2_CLIENT_SCOPE"] ?? "";
if (oauth2ClientScope.trim().length < 1) {
  throw new Error("Oauth2 client scope missing");
}
const clientScopes: string[] = oauth2ClientScope.split(",");

const oauth2ClientSecret: string = process.env["OAUTH2_CLIENT_SECRET"] ?? "";
if (oauth2ClientSecret.trim().length < 1) {
  throw new Error("Oauth2 client secret missing");
}

const oauth2Client = new OAuth2Client({
  server: oauth2BaseUri,
  clientId: oauth2ClientId,
  clientSecret: oauth2ClientSecret,
});

let oauth2FetchClient: OAuth2Fetch | null;

function createOauth2HttpClient(logger: Logger): OAuth2Fetch {
  return new OAuth2Fetch({
    client: oauth2Client,
    getNewToken: async () => {
      return await oauth2Client.clientCredentials({ scope: clientScopes });
    },
    onError: (err: Error) => {
      logger.error("OAuth2Fetch error", err);
    },
    scheduleRefresh: false,
  });
}

function getOauth2HttpClient(logger: Logger): OAuth2Fetch {
  if (!oauth2FetchClient) {
    oauth2FetchClient = createOauth2HttpClient(logger);
  }

  return oauth2FetchClient;
}

const fetch =
  (logger: Logger) =>
  async (input: RequestInfo, init?: RequestInit): Promise<Response> => {
    const httpClient: OAuth2Fetch = getOauth2HttpClient(logger);

    try {
      const response: Response = await httpClient.fetch(input, init);
      if (!response.ok) {
        throw new Error(`Response status: ${response.status.toString()} ${response.statusText}`);
      }

      logger.info("Fetch API response ok", response.ok.toString());
      logger.info("Fetch API response status", response.status.toString());
      logger.info("Fetch API response status text", response.statusText);
      logger.info("Fetch API response type", response.type);
      logger.info("Fetch API response url", response.url);
      logger.info("Fetch API response redirected", response.redirected.toString());

      /*
      const json = await response.json();
      logger.info("Fetch API response json", JSON.stringify(json));
      */

      return response;
    } catch (err) {
      logger.error("Fetch exception", err instanceof Error ? err : JSON.stringify(err));
      throw err;
    }
  };

export const createOauth2ApiService = (logger: Logger): IApiService => {
  return {
    fetch: fetch(logger),
  };
};
