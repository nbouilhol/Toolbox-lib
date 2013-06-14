namespace Utilities.SqlHelpers
{
    public enum EnumAlias
    {
        NULL = 0,
        DEV = 1,
        PRE = 2,
        PROD = 3
    }

    public static class EnumAliasHelper
    {
        public const string Alias = "EnumAlias";

        public static EnumAlias ConvertBuildConfigurationToEnumAlias()
        {
#if (DEBUG)
            return EnumAlias.DEV;
#elif (RELEASE)
			return EnumAlias.PROD;
#elif (PRE)
			return EnumAlias.PRE;
#endif
        }
    }
}