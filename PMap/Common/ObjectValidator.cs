using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Web;
    using System.Xml.Serialization;

    public static class ObjectValidator
    {

        public class ValidationError
        {
            public string Field { get; set; }
            public string Message { get; set; }
        }


        public static List<ValidationError> ValidateObject(object p_obj,  Type[] p_attrfilter = null)
        {
            List<ValidationError> Errors = new List<ValidationError>();
            Type tp = p_obj.GetType();
            PropertyInfo[] writeProps;
            if (p_attrfilter == null)
                writeProps = tp.GetProperties();
            else
                writeProps = tp.GetProperties().Where(pi => p_attrfilter.Count(px => Attribute.IsDefined(pi, px)) > 0).ToArray();

            foreach (var propInf in writeProps.Where( w=>w.CanWrite))
            {
                try
                {

                                      
                    var context = new ValidationContext(p_obj, null, null);
                    context.MemberName = propInf.Name;
                    var results = new List<ValidationResult>();
                    var v = propInf.GetValue(p_obj, null);
                    var isValid = Validator.TryValidateProperty(v, context, results);

                    if (!isValid)
                    {

                        foreach (var validationResult in results)
                        {
                            ValidationError err = new ValidationError()
                            {
                                Field = p_obj.GetType().Name + "." + validationResult.MemberNames.First(),
                                Message = validationResult.ErrorMessage
                            };
                            Errors.Add(err);
                        }
                    }
                    else
                    {

                        if (v!= null && v.GetType() == typeof(DateTime) && v.ToString() == DateTime.MinValue.ToString())
                        {
                            var required = Attribute.IsDefined(propInf, typeof(RequiredAttribute));
                            if (required)
                            {
                                ValidationError err = new ValidationError()
                                {
                                    Field = context.MemberName,
                                    Message = $"The {context.MemberName} field is required."
                                };
                                Errors.Add(err);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return Errors;
        }
    }
}
