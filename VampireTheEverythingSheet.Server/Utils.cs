namespace VampireTheEverythingSheet.Server
{
    public static class Utils
    {
        public static int? TryGetInt(object? input)
        {
            if(input == null)
            {
                return null;
            }
            if(
                (input is int intVal) || 
                (input is string stringVal && int.TryParse(stringVal, out intVal)) ||
                int.TryParse(input.ToString(), out intVal)
              )
            {
                return intVal;
            }
            return null;
        }

        public static bool TryGetInt(object? input, out int result)
        {
            int? trueResult = TryGetInt(input);
            result = trueResult ?? 0;
            return trueResult != null;
        }
    }
}
