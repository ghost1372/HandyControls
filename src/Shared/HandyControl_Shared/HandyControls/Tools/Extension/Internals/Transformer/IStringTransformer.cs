﻿namespace HandyControl.Tools.Extension;

/// <summary>
/// Can tranform a string
/// </summary>
public interface IStringTransformer
{
    /// <summary>
    /// Transform the input
    /// </summary>
    /// <param name="input">String to be transformed</param>
    /// <returns></returns>
    string Transform(string input);
}
