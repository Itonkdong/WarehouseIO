using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseIO.ControlClasses
{
    public class ErrorHandler
    {
        public const string MISSING_INPUTUS_ERROR_MESSAGE = "Fill all the inputs.";

        public static class ErrorMessages
        {
            public const string MUST_HAVE_AT_LEAST_TWO_WAREHOUSES = "You must be a part of at least two warehouses to make a transfer. Create one or become operator of another.";
            public const string MUST_HAVE_AT_LEAST_ONE_WAREHOUSES = "You must be a part of at least one warehouse to add manage operatos. Create one or become operator of another.";
            public const string TRANSFER_HAS_NO_ITEMS = "You can not make a transfer without including any items";
        }

        public static string MissingInputsErrorMessage<T>(ViewDataDictionary<T> viewData, List<string> propertyNames)
        {
            return propertyNames
                .Exists(propertyName =>
                {
                    var errorMessage = viewData.ModelState[propertyName]?.Errors.FirstOrDefault()?.ErrorMessage;
                    return !string.IsNullOrEmpty(errorMessage);
                })
                ? MISSING_INPUTUS_ERROR_MESSAGE
                : "";
        }

        public static string GetValidationSummaryFullErrorMessage<T>(ViewDataDictionary<T> viewData, List<string> propertyNames)
        {
            return string.Join(" ", propertyNames
                                .Select(propertyName =>
                                {
                                    var errorMessage = viewData.ModelState[propertyName]?.Errors.FirstOrDefault()?.ErrorMessage;
                                    return errorMessage;
                                })
                                .Where(errorMessage => !string.IsNullOrEmpty(errorMessage)));
        }

        public static bool DoesErrorExist(params string[] errorMessages)
        {
            return errorMessages
                .ToList()
                .Exists(errorMessage => !string.IsNullOrEmpty(errorMessage));
        }

        public static string GetFirstExistingError(params string[] errorMessages)
        {
            return errorMessages
                .ToList()
                .First(errorMessage => !string.IsNullOrEmpty(errorMessage));
        }
    }
}