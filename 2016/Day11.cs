using System.Text.RegularExpressions;

namespace AOC.AOC2016;

public partial class Day11 : Day<Day11.Facility>
{
    protected override string? SampleRawInput { get => "The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.\nThe second floor contains a hydrogen generator.\nThe third floor contains a lithium generator.\nThe fourth floor contains nothing relevant."; }

    public class Facility
    {
        public required List<Floor> Floors;
        public required int Elevator;

        public List<Move> GetValidMoves()
        {
            var moves = new List<Move>();

            // we can move up or down one floor, taking up to two items with us.  if we take one of each they must be of the same type.
            // the resulting items on the destination floor are subject to the rules too - we can't leave a microchip on the same floor as a generator unless the generator is compatible with the microchip.
            var currentFloor = Floors[Elevator - 1];

            // move up
            if (Elevator < 4)
            {
                // can always move up with nothing.
                moves.Add(new Move() { ElevatorFrom = Elevator, ElevatorTo = Elevator + 1 });

                // from the items that are currently on this floor, determine the remaining moves.
                if (currentFloor.Microchips.Count >= 2)
                {
                    // check all combinations of two microchips.
                    foreach (var chip1 in currentFloor.Microchips)
                    {
                        foreach (var chip2 in currentFloor.Microchips)
                        {
                            if (chip1 == chip2) continue;
                            // check if moving these up would make that floor invalid.
                            var nextFloor = Floors[Elevator];
                            if (nextFloor.Generators.Contains(chip1) || nextFloor.Generators.Contains(chip2)) continue;


                            moves.Add(new Move() { ElevatorFrom = Elevator, ElevatorTo = Elevator + 1, Microchips = new HashSet<string>() { chip1, chip2 } });
                        }
                    }
                }
                var mc = currentFloor.Microchips;
            }

            return moves;
        }
    }

    public class Floor
    {
        public required HashSet<string> Microchips;
        public required HashSet<string> Generators;
    }

    public class Move
    {
        public required int ElevatorFrom;
        public required int ElevatorTo;
        public HashSet<string> Microchips = [];            // 0, 1, or 2
        public HashSet<string> Generators = [];            // 0, 1, or 2; but not more than 2 items total, and if one of each, the types must match
    }

    protected override Answer Part1()
    {
        // find the least number of elevator stops to bring everything to the fourth floor.  the elevator can carry at most two items.  microchips cannot be on the same floor as a generator (or both in the elevator) unless the generator is compatible with the microchip.
        // each elevator floor is a step (e.g. going from F1 to F3 is two steps regardless of what was carried or if there was nothing to do on F2).

        // each step is defined by the current elevator position, whether it is moving up or down, and the 0,1, or 2 items being carried.
        // the desired end state is everything on the 4th floor.

        // we can use a search algorithm to find the end state, with each node representing the current state of the facility (so we don't revisit).  we can use a priority queue to keep the most promising nodes at the front of the queue.
        // this looks a lot like A* search.

        return 0;
    }

    protected override Answer Part2()
    {
        throw new NotImplementedException();
    }

    protected override Facility Parse(string input)
    {
        var floors = new List<Floor>();

        foreach (var line in input.Split("\n").Where(p => p != ""))
        {
            var microchips = new HashSet<string>();
            var generators = new HashSet<string>();

            var chips = MicrochipRegex().Matches(line);
            var gens = GeneratorRegex().Matches(line);
            foreach (var match in chips.ToList())
            {
                microchips.Add(match.Groups[1].Value);
            }
            foreach (var match in gens.ToList())
            {
                generators.Add(match.Groups[1].Value);
            }

            floors.Add(new Floor() { Microchips = microchips, Generators = generators });
        }

        return new Facility() { Floors = floors, Elevator = 1 };
    }

    [GeneratedRegex("a (.+?)-compatible microchip")]
    private static partial Regex MicrochipRegex();
    [GeneratedRegex("a (.+?) generator")]
    private static partial Regex GeneratorRegex();
}