/*
The MIT License (MIT)
Copyright (c) 2012 Denys Vuika

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace WorkflowDesigner.Sdk
{
  internal static class Strings
  {
    public const string ArgumentExceptionEmptyString = @"'{0}' cannot be an empty string ("").";
    public const string ArgumentExceptionEmptyStream = @"'{0}' cannot be an empty stream.";
    public const string ArgumentExceptionCannotReadStream = @"'{0}' cannot read stream.";
    public const string InternalExceptionMessage = @"Internal error occurred. Additional information: '{0}'.";
    public const string ArgumentExceptionType = @"The type of '{0}' is not supported as an argument.";
  }

  public static class Requires
  {
    [DebuggerStepThrough]
    public static void NotNull<T>(T value, string parameterName) where T : class
    {
      if (value == null) throw new ArgumentNullException(parameterName);
    }

    [DebuggerStepThrough]
    public static void NotNullOrEmpty(string value, string parameterName)
    {
      NotNull(value, parameterName);

      if (value.Length == 0)
        throw new ArgumentException(
          string.Format(CultureInfo.CurrentCulture, Strings.ArgumentExceptionEmptyString, parameterName), parameterName);
    }

    [DebuggerStepThrough]
    public static void NotNullOrEmpty(Stream value, string parameterName)
    {
      NotNull(value, parameterName);

      if (!value.CanRead)
        throw new ArgumentException(
          string.Format(CultureInfo.CurrentCulture, Strings.ArgumentExceptionCannotReadStream, parameterName), parameterName);

      if (value.Length == 0)
        throw new ArgumentException(
          string.Format(CultureInfo.CurrentCulture, Strings.ArgumentExceptionEmptyStream, parameterName), parameterName);
    }

    [DebuggerStepThrough]
    public static void NotNullOrWhiteSpace(string value, string parameterName)
    {
      NotNull(value, parameterName);

      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException(
          string.Format(CultureInfo.CurrentCulture, Strings.ArgumentExceptionEmptyString, parameterName), parameterName);
    }

    [DebuggerStepThrough]
    public static void NotEmpty(Guid value, string parameterName)
    {
      if (Guid.Empty.Equals(value))
        throw new ArgumentException(parameterName);
    }

    [DebuggerStepThrough]
    public static void NotEmpty(string value, string parameterName)
    {
      NotNullOrWhiteSpace(value, parameterName);
    }

    [DebuggerStepThrough]
    public static void OfType<T>(object value, string parameterName) where T : class
    {
      NotNull(value, parameterName);

      var type = value as Type;
      if (type != null)
      {
        if (!typeof(T).IsAssignableFrom(type))
          throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentExceptionType, parameterName), parameterName);
      }
      else if (!(value is T))
        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentExceptionType, parameterName), parameterName);
    }
  }
}
