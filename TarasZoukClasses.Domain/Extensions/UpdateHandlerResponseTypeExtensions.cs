﻿namespace TarasZoukClasses.Domain.Extensions
{
    using TarasZoukClasses.Domain.Handlers;

    public static class UpdateHandlerResponseTypeExtensions
    {
        public static bool IsOk(this UpdateHandlerResponseType updateHandlerResponseType)
        {
            return updateHandlerResponseType == UpdateHandlerResponseType.Ok;
        }

        public static bool IsError(this UpdateHandlerResponseType updateHandlerResponseType)
        {
            return updateHandlerResponseType == UpdateHandlerResponseType.Error;
        }
    }
}