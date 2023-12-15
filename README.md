# Advent of Code

My C# implementations of http://adventofcode.com puzzle solutions.

Sample inputs are included for completed days as hardcoded strings in each `Day` implementation.

Real inputs are user-specific and are not committed per the TOS.  To run with (your) real input, download it and save it to the appropriate `/[year]/inputs` folder with the name `Day[N].txt`, e.g. `/2023/inputs/Day1.txt`.

Requires .NET 7 runtime.

Usages
```
dotnet run [year]
```
Run all implemented puzzle solutions for the given year (or current year if not provided), using saved real inputs from `[current year]/inputs/Day*.txt` only, and show total runtime.
```
dotnet run [year] [day]
```
Run specific [day] of the given [year] solution of the current year (sample plus saved input from `[year]/inputs/Day[day].txt`) or scaffold an empty implementation to `[year]/Day[day].cs` if not yet implemented.



