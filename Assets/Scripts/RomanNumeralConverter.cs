﻿using System;

public static class RomanNumeralConverter
{
    // ---- Array of Roman numerals for the upgrade screen ----
    
    private static readonly string[] RomanNumerals = { "I", "II", "III", "IV" };

    public static string ToRoman(int number)
    {
        if (number >= 1 && number <= RomanNumerals.Length)
        {
            return RomanNumerals[number - 1];
        }
        return string.Empty;
    }
}