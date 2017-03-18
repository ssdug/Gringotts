(function ($) {
    var mvcvalidationextensions = function () {
        var init = function () {
            var greaterThanEqual = 'greaterthanequalto',
                greaterThan = 'greaterthan',
                lessThanEqual = 'lessthanequalto',
                lessThan = 'lessthan',
                requiredIf = 'requiredif',
                requiredIfValue = 'requiredifvalue',
                requiredIfAnyValue = 'requiredifanyvalue';
            var comparisonParams = ['otherproperty'];
            var comparisonValueParams = ['otherproperty', 'othervalue'];

            var customValidators = [
                { type: greaterThanEqual, params: comparisonParams },
                { type: greaterThan, params: comparisonParams },
                { type: lessThanEqual, params: comparisonParams },
                { type: lessThan, params: comparisonParams },
                { type: requiredIf, params: comparisonParams },
                { type: requiredIfValue, params: comparisonValueParams },
                { type: requiredIfAnyValue, params: comparisonValueParams }
            ];

            // Taken from jquery.validate.unobtrusive because this was better than what we were originally doing.
            var getModelPrefix = function (fieldName) {
                return fieldName.substr(0, fieldName.lastIndexOf('.') + 1);
            };

            var appendModelPrefix = function (value, prefix) {
                if (value.indexOf('*.') === 0) {
                    value = value.replace('*.', prefix);
                }
                return value;
            };

            var escapeAttributeValue = function (value) {
                // As mentioned on http://api.jquery.com/category/selectors/
                return value.replace(/([!"#$%&'()*+,./:;<=>?@\[\\\]^`{|}~])/g, '\\$1');
            };

            var setValidationValues = function (options, ruleName, value) {
                options.rules[ruleName] = value;
                if (options.message) {
                    options.messages[ruleName] = options.message;
                }
            };
            // Thanks jquery.validate.unobtrusive

            var getObjectFromElement = function (element) {
                return $(element).is(':radio') || $(element).is(':checkbox') ? $('input[name=' + element.name + ']:checked') : $(element);
            };

            var compare = function (obj, otherObj, comparisonType) {
                switch (comparisonType) {
                    case greaterThanEqual:
                        return obj >= otherObj;

                    case greaterThan:
                        return obj > otherObj;

                    case lessThanEqual:
                        return obj <= otherObj;

                    case lessThan:
                        return obj < otherObj;

                    default:
                        return false;
                }
            };

            var validateComparison = function (element, parameter, comparisonType) {
                var obj = $(element);

                if (obj.is(':not([data-val-required])') && (obj.val() == null || obj.val() == '')) {
                    return true;
                }

                var otherObj = $(parameter);

                // date compare
                if (obj.attr('data-val-date') != null) {
                    var date = Date.parse(obj.val());
                    var otherDate = Date.parse(otherObj.val());

                    if (!(isNaN(date) || isNaN(otherDate))) {
                        return compare(date, otherDate, comparisonType);
                    }

                    return false;
                }

                // numeric compare
                if (!(isNaN(parseFloat(obj.val())) || parseFloat(isNaN(otherObj.val())))) {
                    return compare(parseFloat(obj.val()), parseFloat(otherObj.val()), comparisonType);
                }

                return false;
            };

            var validateRequiredIf = function (element, parameter) {
                var obj = $(element);
                var otherObj = getObjectFromElement(parameter);

                if (otherObj.val() == null || otherObj.val() == undefined || otherObj.val() === '') {
                    return true;
                }

                return (obj.val() != null && obj.val() != undefined && obj.val() !== '');
            };

            var validateRequiredIfValue = function (element, params) {
                var obj = $(element);
                var otherObj = getObjectFromElement(params.element);

                if (otherObj.val() == null || otherObj.val() == undefined || otherObj.val() === '') {
                    return true;
                }

                return params.othervalue === otherObj.val() ?
                    obj.val() != null && obj.val() != undefined && obj.val() !== '' : true;
            };

            var validateRequiredIfAnyValue = function (element, params) {
                var obj = $(element);
                var otherObj = getObjectFromElement(params.element);

                if (otherObj.val() == null || otherObj.val() == undefined || otherObj.val() === '') {
                    return true;
                }

                return $.inArray(otherObj.val(), JSON.parse(params.othervalue)) >= 0 ?
                    obj.val() != null && obj.val() != undefined && obj.val() !== '' : true;
            };

            // setup our comparison adapters
            for (var i = 0; i < customValidators.length; i++) {
                $.validator.unobtrusive.adapters.add(customValidators[i].type, customValidators[i].params,
                (function (i) {
                    return function (options) {
                        var prefix = getModelPrefix(options.element.name),
                            otherProperty = options.params.otherproperty,
                            fullOtherName = appendModelPrefix(otherProperty, prefix),
                            element = $(options.form).find(':input').filter("[name='" + escapeAttributeValue(fullOtherName) + "']")[0];

                        if ($(element).is(':hidden') && options.message != null) {
                            options.message = options.message.replace(otherProperty, $(element).val());
                        }

                        setValidationValues(options, customValidators[i].type, customValidators[i].params.length == 1 ? element : { element: element, othervalue: options.params.othervalue });
                    };
                }(i)));
            }

            $.validator.addMethod(greaterThanEqual, function (value, element, params) {
                return validateComparison(element, params, greaterThanEqual);
            });

            $.validator.addMethod(greaterThan, function (value, element, params) {
                return validateComparison(element, params, greaterThan);
            });

            $.validator.addMethod(lessThanEqual, function (value, element, params) {
                return validateComparison(element, params, lessThanEqual);
            });

            $.validator.addMethod(lessThan, function (value, element, params) {
                return validateComparison(element, params, lessThan);
            });

            $.validator.addMethod(requiredIf, function (value, element, params) {
                return validateRequiredIf(element, params);
            });

            $.validator.addMethod(requiredIfValue, function (value, element, params) {
                return validateRequiredIfValue(element, params);
            });

            $.validator.addMethod(requiredIfAnyValue, function (value, element, params) {
                return validateRequiredIfAnyValue(element, params);
            });
        };

        return {
            init: init
        };
    }();

    mvcvalidationextensions.init();

})(jQuery);