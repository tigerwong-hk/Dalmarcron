import { Logger } from "@aws-lambda-powertools/logger";
import { OAuth2Client, OAuth2Fetch } from "@badgateway/oauth2-client";

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

export const createOauth2ApiService = (logger: Logger): OAuth2Fetch => {
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
};
