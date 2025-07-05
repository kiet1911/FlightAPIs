namespace FlightAPIs.Helper
{
    internal static class JsonConvert
    {
        public static String ToJson(this object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }   
    }
}
