using System.Net;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // Update with your Mobile App url!
    MobileServiceClient client = new MobileServiceClient("INSERT_YOUR_MOBILE_APP_URL_HERE");
    dynamic data = await req.Content.ReadAsStringAsync();

    JArray arrayJson;
    dynamic authToken;
    string tableName;
    dynamic objectToInsert;
    try
    {
        // Server expects a json arrary with the format:
        // [{"access_token":"value"},{"tableName":"value"},{instanceJson}]
        arrayJson = JArray.Parse(data);
        authToken = arrayJson[0];
        tableName = arrayJson[1].Value<string>("tableName");
        objectToInsert = arrayJson[2];
    }
    catch (Exception exception)
    {
        return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
    }
    
    // Try to log in. Return Unauthorized response upon failure.
    MobileServiceUser user = null;
    while (user == null)
    {
        try
        {
            // Change MobileServiceAuthenticationProvider.Facebook
            // to MobileServiceAuthenticationProvider.Google if using Google auth.
            user = await client.LoginAsync(MobileServiceAuthenticationProvider.Facebook, authToken);                
        }
        catch (InvalidOperationException exception)
        {
            log.Info("Log in failure!");
            return req.CreateErrorResponse(HttpStatusCode.Unauthorized, exception);
        }
    }
    // Try to Insert. Return BadRequest response upon failure.
    try
    {
        JToken insertedObject = await client.GetTable(tableName).InsertAsync(objectToInsert);
        return  req.CreateResponse(HttpStatusCode.OK, insertedObject);
    }
    catch (Exception exception)
    {
        return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
    }
}