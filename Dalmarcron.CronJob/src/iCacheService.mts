export interface ICacheService {
  getString(cacheKey: string, numTries?: number): Promise<string | null>;

  keyExists(cacheKey: string, numTries?: number): Promise<boolean>;

  removeKey(cacheKey: string, numTries?: number): Promise<boolean>;

  setString(
    cacheKey: string,
    value: string,
    expireSeconds?: number,
    numTries?: number,
  ): Promise<string | null>;
}
