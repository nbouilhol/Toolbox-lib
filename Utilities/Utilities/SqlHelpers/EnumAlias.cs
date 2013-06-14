namespace Utilities.SqlHelpers
{
    public enum EnumAlias
    {
        Null = 0,
        Dev = 1,
        Pre = 2,
        Prod = 3
    }

    public static class EnumAliasHelper
    {
        public const string Alias = "EnumAlias";

        public static EnumAlias ConvertBuildConfigurationToEnumAlias()
        {
#if (DEBUG)
            return EnumAlias.Dev;
#elif (RELEASE)
			return EnumAlias.PROD;
#elif (PRE)
			return EnumAlias.PRE;
#endif
        }
    }
}