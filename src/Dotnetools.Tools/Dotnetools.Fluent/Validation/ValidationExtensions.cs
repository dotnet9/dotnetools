using System.Linq.Expressions;
using Dotnetools.Models;

namespace Dotnetools.Fluent.Validation;

public delegate void ValidateMethod(IValidationErrors errors);

public static class ValidationExtensions
{
	public static void ValidateProperty<TSender, TRet>(this TSender viewModel, Expression<Func<TSender, TRet>> property, ValidateMethod validateMethod) where TSender : IRegisterValidationMethod
	{
		string propertyName = ((MemberExpression)property.Body).Member.Name;

		viewModel.RegisterValidationMethod(propertyName, validateMethod);
	}
}