using System.Configuration.Assemblies;
using System.Data;

namespace AOC.AOC2023;

public class Day19 : Day<Day19.PartClassifier>
{
    protected override string? SampleRawInput { get => "px{a<2006:qkq,m>2090:A,rfg}\npv{a>1716:R,A}\nlnx{m>1548:A,A}\nrfg{s<537:gd,x>2440:R,A}\nqs{s>3448:A,lnx}\nqkq{x<1416:A,crn}\ncrn{x>2662:A,R}\nin{s<1351:px,qqz}\nqqz{s>2770:qs,m<1801:hdj,R}\ngd{a>3333:R,R}\nhdj{m>838:A,pv}\n\n{x=787,m=2655,a=1222,s=2876}\n{x=1679,m=44,a=2067,s=496}\n{x=2036,m=264,a=79,s=2244}\n{x=2461,m=1339,a=466,s=291}\n{x=2127,m=1623,a=2188,s=1013}"; }

    public class PartClassifier
    {
        public required WorkflowTreeNode Classifier { get; set; }

        public required List<Part> Parts { get; set; }

        // accept or reject a part by walking the classifier tree until we reach an accept/reject node
        public bool ClassifyPart(Part part)
        {
            var node = Classifier;
            while (true)
            {
                if (node.Node.Evaluate(part))
                {
                    if (node.IfTrue == null) return (bool)node.Node.Accept!;
                    node = node.IfTrue;
                }
                else
                {
                    if (node.IfFalse == null) return (bool)node.Node.Accept!;
                    node = node.IfFalse;
                }
            }
        }
    }

    public class WorkflowTreeNode
    {
        public WorkflowTreeNode? Parent { get; set; }
        public required WorkflowRule Node { get; set; }
        public WorkflowTreeNode? IfTrue { get; set; }
        public WorkflowTreeNode? IfFalse { get; set; }
    }

    private class Workflow
    {
        public required string Name { get; set; }
        public required List<WorkflowRule> Rules { get; set;}
    }
    
    public abstract class WorkflowRule
    {
        public bool? Accept { get; set; }
        public string? NextWorkflow { get; set; }
        public WorkflowRule? IfFalseNextRule { get; set; }         // navigation property, points to next element in List<WorkflowRule> of the current workflow so we can easily navigate when building the tree.
        public abstract bool Evaluate(Part part);
    }

    public class ConditionalWorkflowRule : WorkflowRule
    {
        public required char PartRatingType { get; set; }
        public char Inequality { get; set; }
        public int Rating { get; set; }

        public override bool Evaluate(Part part)
        {
            var partRating = part.Ratings.Single(p => p.PartRatingType == PartRatingType).Rating;
            return (Inequality == '<' && partRating < Rating) || (Inequality == '>' && partRating > Rating);
        }
    }

    public class DefaultWorkflowRule : WorkflowRule
    {
        public override bool Evaluate(Part part) => true;
    }

    public class Part
    {
        public required List<PartRating> Ratings { get; set; }
    }

    public class PartRating
    {
        public char PartRatingType { get; set; }
        public int Rating { get; set; }
    }

    protected override Answer Part1()
    {
        return Input.Parts.Where(Input.ClassifyPart).Sum(p => p.Ratings.Sum(q => q.Rating));
    }

    protected override Answer Part2()
    {
        // my second implementation after looking over the solution thread.
        // instead of an overly-complicated negative approach (finding all of the rejected ranges with bottom-up tree traversal, having to intersect them, and subtracting them from the whole space of possible part ratings),
        // this is a positive approach summing the volumes with a simple top-down tree traversal of a binary k-d tree (k=4, d=4000).
        // I did realize the rules were best represented by a tree and built a tree in my own first implementation,
        // but didn't realize how easy it was to aggregate what we need in this way, or that it is called a k-d(imensional) tree.
        // what a handy structure.
        // it is worth noting that the negative approach also works with this simple top-down traversal, by subtracting the sum of the rejected volumes from the total 4000^4 space,
        // but it is an unnecessary extra step.
        var keys = new char[] {'x','m','a','s'};
        var hypercube = keys.ToDictionary(p => p, p => (low: 1, high: 4000));
        return SumAcceptedNodes(Input.Classifier, hypercube);

        // my first implementation (all mine, took most of the day):
        // find the (disjoint) sets of part ratings that are rejected by each path that results in a rejection.
        // then substract those from the whole space of possible part ratings (4000^4).

        /*var rejectedLeafNodes = new List<WorkflowTreeNode>();
        FindRejectedLeafNodes(Input.Classifier, rejectedLeafNodes);

        // each path results in a single range of rejected part ratings for each part rating type (x,m,a,s).  if there are multiple conditions on a part rating type, the range gets intersected.
        var rejected = new List<Dictionary<char, (int low, int high)>>();

        // walk each leaf node back to the root, keeping track of the intersected rejected range for each part rating type
        foreach (var node in rejectedLeafNodes)
        {
            // start by rejecting everything, as we run into conditional rules we'll intersect the current rejected ranges with the range rejected by that rule.
            var rejectedRanges = keys.ToDictionary(p => p, p => (low: 1, high: 4000));
            
            var currentNode = node;
            WorkflowTreeNode? childNode = null;         // keep track of which path we took (ifTrue or ifFalse) so we know how to construct the rejected range
            while (currentNode != null)
            {
                if (currentNode.Node is ConditionalWorkflowRule cond)
                {
                    var partRatingType = cond.PartRatingType!;

                    // range rejected by following this rule (careful of the inequality and which path we took!)
                    var range = currentNode.IfFalse == childNode
                            ? (cond.Inequality == '<' ? (low: cond.Rating, high: 4000) : (low: 1, high: cond.Rating))
                            : (cond.Inequality == '<' ? (low: 1, high: cond.Rating-1) : (low: cond.Rating+1, high: 4000));

                    // intersect
                    rejectedRanges[partRatingType] = (Math.Max(rejectedRanges[partRatingType].low, range.low), Math.Min(rejectedRanges[partRatingType].high, range.high));
                }

                childNode = currentNode;
                currentNode = currentNode.Parent;
            }

            rejected.Add(rejectedRanges);
        }

        // now we have a list of all the ranges that are rejected by each path (one range per x,m,a,s per path)
        // we need to not double-count any rejected-range lists that are subsets of other rejected-range lists.
        // handle both supersets (replace the superset with the subset) and subsets (ignore the subset).
        // if neither a superset nor a subset, add it to the final list and continue.

        var finalRejected = new List<Dictionary<char, (int low, int high)>> { rejected[0] };      // start with the first set of rejected ranges

        for (var i=1; i<rejected.Count; i++)
        {
            var j = 0;
            for (; j<finalRejected.Count; j++)
            {
                var isSuperset = true;
                var isSubset = true;
                foreach (var key in keys)
                {
                    var thisRange = rejected[i].ContainsKey(key) ? rejected[i][key] : (low: 1, high: 4000);
                    var otherRange = finalRejected[j].ContainsKey(key) ? finalRejected[j][key] : (low: 1, high: 4000);
                    
                    if (thisRange.low > otherRange.low || thisRange.high < otherRange.high) isSuperset = false;
                    if (thisRange.low < otherRange.low || thisRange.high > otherRange.high) isSubset = false;
                }

                if (isSuperset)                     // if this range is a superset of the other range, replace the other range with this one in the final list
                {
                    finalRejected[j] = rejected[i];
                    break;
                }
                else if (isSubset) break;           // if this range is a subset of the other range, ignore it so we don't double-count
            }

            if (j == finalRejected.Count)           // neither a superset nor a subset; add it to the final list
                finalRejected.Add(rejected[i]);
        }

        // subtract the sets of rejected ranges from the whole space of possible part ratings (4000^4)
        var valid = (long)Math.Pow(4000, 4);

        foreach (var rej in finalRejected)
        {
            valid -= rej.Aggregate(1L, (a, b) => a * (b.Value.high - b.Value.low + 1));
        }

        return valid;*/
    }

    // binary k-d tree traversal (sum 4-dimensional volume at accepted leaf nodes)
    private long SumAcceptedNodes(WorkflowTreeNode node, Dictionary<char, (int low, int high)> hypercube)
    {
        if (node == null) return 0;

        // non-conditional node pointing to indicated workflow, next node is the first rule of that workflow
        if (node.Node is DefaultWorkflowRule def)
        {
            if (node.Node.Accept == true) return hypercube.Values.Aggregate(1L, (a, b) => a * (b.high - b.low + 1));
            return SumAcceptedNodes(node.IfTrue!, hypercube);
        }

        // conditional node
        var cond = (ConditionalWorkflowRule)node.Node;
        var partRatingType = cond.PartRatingType;
        var inequality = cond.Inequality;
        var rating = cond.Rating;

        var ifTrueHypercube = new Dictionary<char, (int, int)>(hypercube);
        var ifFalseHypercube = new Dictionary<char, (int, int)>(hypercube);

        if (inequality == '<')
        {
            ifTrueHypercube[partRatingType] = (hypercube[partRatingType].low, rating-1);
            ifFalseHypercube[partRatingType] = (rating, hypercube[partRatingType].high);
        }
        else
        {
            ifTrueHypercube[partRatingType] = (rating+1, hypercube[partRatingType].high);
            ifFalseHypercube[partRatingType] = (hypercube[partRatingType].low, rating);
        }

        return SumAcceptedNodes(node.IfTrue!, ifTrueHypercube) + SumAcceptedNodes(node.IfFalse!, ifFalseHypercube);
    }

    private void FindRejectedLeafNodes(WorkflowTreeNode node, List<WorkflowTreeNode> rejectedLeafNodes)
    {
        if (node.Node.Accept == false) rejectedLeafNodes.Add(node);
        if (node.IfTrue != null) FindRejectedLeafNodes(node.IfTrue, rejectedLeafNodes);
        if (node.IfFalse != null) FindRejectedLeafNodes(node.IfFalse, rejectedLeafNodes);
    }

    protected override PartClassifier Parse(RawInput input)
    {
        var workflows = new List<Workflow>();
        var parts = new List<Part>();

        var sections = input.LineGroups();
        foreach (var workflow in sections[0])
        {
            var wfSection = workflow.Split("{");
            var workflowName = wfSection[0];
            var workflowRules = new List<WorkflowRule>();
            foreach (var rule in wfSection[1].Trim('}').Split(',').Where(p => p != ""))
            {
                var ruleParts = rule.Split(':');
                var predicateParts = ruleParts[0].Split('<','>');
                var partRatingType = ruleParts.Length > 1 ? predicateParts[0][0] : (char?)null;
                var inequality = ruleParts.Length > 1 ? (ruleParts[0].Contains('<') ? '<' : ruleParts[0].Contains('>') ? '>' : throw new Exception("Unknown predicate!")) : (char?)null;
                var rating = predicateParts.Length > 1 ? int.Parse(predicateParts[1]) : (int?)null;
                var nextWorkflow = ruleParts.Length > 1 ? ruleParts[1] : rule;
                var isAcceptRejectRule = nextWorkflow == "A" ? true : nextWorkflow == "R" ? false : (bool?)null;

                workflowRules.Add(
                    partRatingType != null ?
                        new ConditionalWorkflowRule { PartRatingType = (char)partRatingType, Inequality = (char)inequality!, Rating = (int)rating!, Accept = isAcceptRejectRule, NextWorkflow = isAcceptRejectRule != null ? null : nextWorkflow }
                        : new DefaultWorkflowRule { Accept = isAcceptRejectRule, NextWorkflow = isAcceptRejectRule != null ? null : nextWorkflow }
                );
            }
            
            // set IfFalseNextRule navigation property for use when building the tree.
            for (var i=0; i<workflowRules.Count-1; i++ ) { workflowRules[i].IfFalseNextRule = workflowRules[i+1]; }
            var wf = new Workflow { Name = workflowName, Rules = workflowRules };

            workflows.Add(wf);
        }

        foreach (var part in sections[1])
        {
            var partRatings = new List<PartRating>();
            foreach (var rating in part.Trim('{','}').Split(','))
            {
                partRatings.Add(new PartRating { PartRatingType = rating[0], Rating = int.Parse(rating[2..]) });
            }
            parts.Add(new Part { Ratings = partRatings });
        }

        return new PartClassifier() {
            Parts = parts,
            Classifier = BuildTree(new WorkflowTreeNode { Node = workflows.Single(p => p.Name == "in").Rules.First() }, workflows)      // classifier starts at first rule of "in" workflow
        };
    }

    private WorkflowTreeNode BuildTree(WorkflowTreeNode node, List<Workflow> workflows)
    {
        if (node.Node is DefaultWorkflowRule)
        {
            if (node.Node.Accept != null) return node;          // leaf node

            // non-conditional node pointing to indicated workflow, next node is the first rule of that workflow
            var nextWorkflowFirstRule = workflows.Single(p => p.Name == node.Node.NextWorkflow!).Rules.First();
            node.IfTrue = BuildTree(new WorkflowTreeNode { Parent = node, Node = nextWorkflowFirstRule }, workflows);
            return node;
        }
        else        // conditional node
        {
            WorkflowTreeNode ifTrueNode;
            var ifFalseNode = new WorkflowTreeNode { Parent = node, Node = node.Node.IfFalseNextRule! };            // conditional nodes always have a next (if false) rule
            if (node.Node.Accept != null)       // accept/reject if true (create leaf node)
            {
                ifTrueNode = new WorkflowTreeNode { Parent = node, Node = new DefaultWorkflowRule { Accept = node.Node.Accept } };
            }
            else  // go to indicated workflow, next node is the first rule of that workflow
            {
                ifTrueNode = new WorkflowTreeNode { Parent = node, Node = workflows.Single(p => p.Name == node.Node.NextWorkflow!).Rules.First() };
            }

            node.IfTrue = BuildTree(ifTrueNode, workflows);
            node.IfFalse = BuildTree(ifFalseNode, workflows);
        }
        

        return node;
    }
}