namespace Utils
{
    public static class PreprocessingHelper
    {
        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}