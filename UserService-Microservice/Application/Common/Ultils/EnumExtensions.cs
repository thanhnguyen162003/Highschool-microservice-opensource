using Domain.Enumerations;
using System.ComponentModel;
using System.Reflection;

namespace Domain.Common.Ultils
{
	public static class EnumExtensions
	{
		public static bool IsInEnum<TEnum, TProperty>(this string value)
		{
			return Enum.TryParse(typeof(TProperty), value, true, out _)
				   || Enum.IsDefined(typeof(TProperty), value);
		}


		public static bool IsStudentOrTeacher(string? strValue)
		{

			if (strValue == null)
			{
				return true;
			}

			return strValue.Equals(RoleEnum.Student.ToString(), StringComparison.OrdinalIgnoreCase) ||
				   strValue.Equals(RoleEnum.Teacher.ToString(), StringComparison.OrdinalIgnoreCase) ||
				   strValue.Equals(((int)RoleEnum.Teacher).ToString()) ||
				   strValue.Equals(((int)RoleEnum.Student).ToString());
		}

		public static bool IsUser(string? strValue)
		{

			if (strValue == null)
			{
				return false;
			}

			return strValue.Equals(RoleEnum.Student.ToString(), StringComparison.OrdinalIgnoreCase) ||
				   strValue.Equals(RoleEnum.Teacher.ToString(), StringComparison.OrdinalIgnoreCase) ||
				   strValue.Equals(((int)RoleEnum.Teacher).ToString()) ||
				   strValue.Equals(((int)RoleEnum.Student).ToString());
		}

		public static bool IsStudent(string? strValue)
		{

			if (strValue == null)
			{
				return false;
			}

			return strValue.Equals(RoleEnum.Student.ToString(), StringComparison.OrdinalIgnoreCase) ||
				   strValue.Equals(((int)RoleEnum.Student).ToString());
		}

		public static bool IsModerator(string? strValue)
		{

			if (strValue == null)
			{
				return false;
			}

			return strValue.Equals(RoleEnum.Moderator.ToString(), StringComparison.OrdinalIgnoreCase) ||
				   strValue.Equals(((int)RoleEnum.Moderator).ToString());
		}

		public static bool IsTeacher(string? strValue)
		{

			if (strValue == null)
			{
				return false;
			}

			return strValue.Equals(RoleEnum.Teacher.ToString(), StringComparison.OrdinalIgnoreCase) ||
				   strValue.Equals(((int)RoleEnum.Teacher).ToString());
		}

		public static RoleEnum? ConvertToRoleValue(object? value)
		{
			if (value == null)
			{
				return null; // Return null for null inputs
			}

			// Handle integer input directly
			if (value is int intValue && Enum.IsDefined(typeof(RoleEnum), intValue))
			{
				return (RoleEnum)intValue;
			}

			// Handle string input
			if (value is string strValue)
			{
				// Check if string can be parsed as integer and corresponds to a valid enum value
				if (int.TryParse(strValue, out int parsedInt) && Enum.IsDefined(typeof(RoleEnum), parsedInt))
				{
					return (RoleEnum)parsedInt;
				}

				// Convert string to enum name and check if valid
				if (Enum.TryParse<RoleEnum>(strValue, true, out var parsedEnum))
				{
					return parsedEnum;
				}
			}

			// Return null if no valid role found
			return null;
		}

		public static AccountStatus? ConvertToStatusValue(object? value)
		{
			if (value == null)
			{
				return null; // Return null for null inputs
			}

			// Handle integer input directly
			if (value is int intValue && Enum.IsDefined(typeof(AccountStatus), intValue))
			{
				return (AccountStatus)intValue;
			}

			// Handle string input
			if (value is string strValue)
			{
				// Check if string can be parsed as integer and corresponds to a valid enum value
				if (int.TryParse(strValue, out int parsedInt) && Enum.IsDefined(typeof(AccountStatus), parsedInt))
				{
					return (AccountStatus)parsedInt;
				}

				// Convert string to enum name and check if valid
				if (Enum.TryParse<AccountStatus>(strValue, true, out var parsedEnum))
				{
					return parsedEnum;
				}
			}

			// Return null if no valid role found
			return null;
		}


		public static T? ConvertToValue<T>(this string value) where T : struct
		{
			if (value == null)
			{
				return null; // Return null for null inputs
			}

			// Check if string can be parsed as integer and corresponds to a valid enum value
			if (int.TryParse(value, out int parsedInt) && Enum.IsDefined(typeof(T), parsedInt))
			{
				return (T)(object)parsedInt;
			}

			// Convert string to enum name and check if valid
			if (Enum.GetNames(typeof(T)).Contains(value, StringComparer.OrdinalIgnoreCase))
			{
				return (T)Enum.Parse(typeof(T), value, true);
			}

			// Return null if no valid role found
			return null;
		}

        public static string GetDescription<T>(T enumValue) where T : Enum
        {
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());

            if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }

            // Trả về tên của enum value nếu không có description
            return enumValue.ToString();
        }
    }
}
