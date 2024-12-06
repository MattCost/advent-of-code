
public class Day05 : BaseDay
{
    Dictionary<int, List<int>> _orderRules = new();
    List<List<int>> _orders = new();

    public Day05()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line = sr.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            var rule = line.Split('|');
            var first = int.Parse(rule[0]);
            var second = int.Parse(rule[1]);
            if (!_orderRules.ContainsKey(first))
            {
                _orderRules[first] = new();
            }
            _orderRules[first].Add(second);
            line = sr.ReadLine();
        }

        while ((line = sr.ReadLine()) != null)
        {
            _orders.Add(line.Split(',').Select(int.Parse).ToList());
        }

    }

    public override ValueTask<string> Solve_1()
    {
        /*
        for each order
            for each update entry
            if we have rules then confirm anything listed in the rule does not appear before current index
        */
        int output = 0;
        foreach (var order in _orders)
        {
            var valid = true;
            for (int i = 1; i < order.Count; i++)
            {
                var pre = order.GetRange(0, i);
                var current = order[i];
                if (!_orderRules.ContainsKey(current)) continue;
                var rule = _orderRules[current];
                if (rule.Select(ruleEntry => pre.Contains(ruleEntry)).Where(x => x == true).Any())
                {
                    //Invalid rule
                    valid = false;
                    break;
                }
            }
            if (valid)
            {
                var midpoint = order[order.Count / 2];
                output += midpoint;
            }

        }
        return new(output.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int output = 0;
        foreach (var order in _orders)
        {
            var valid = true;
            for (int i = 1; i < order.Count; i++)
            {
                var pre = order.GetRange(0, i);
                var current = order[i];
                if (!_orderRules.ContainsKey(current)) continue;
                var rule = _orderRules[current];
                if (rule.Select(ruleEntry => pre.Contains(ruleEntry)).Where(x => x == true).Any())
                {
                    //Invalid rule
                    valid = false;
                    break;
                }
            }
            if (!valid)
            {
                Console.WriteLine($"Order {string.Join(' ', order)} is invalid");
                var sortedOrder = FixOrder(order);
                Console.WriteLine($"Fixed order {string.Join(' ', sortedOrder)} ");

                var midpoint = sortedOrder[sortedOrder.Count / 2];
                output += midpoint;
            }

        }
        return new(output.ToString());
    }

    private List<int> FixOrder(List<int> order)
    {
        var output = new List<int>();
        output.Add(order.First());
        for (int i = 1; i < order.Count; i++)
        {
            var current = order[i];
            if (!_orderRules.ContainsKey(current))
            {
                //if no rules that we must be before someone else, add to end
                output.Add(current);
                continue;
            }
            var rule = _orderRules[current];
            var anyIssues = rule.Select(ruleEntry => output.IndexOf(ruleEntry)).Where(i => i != -1);
            if (anyIssues.Any())
            {
                var target = anyIssues.Min();
                output.Insert(target, current);
            }
            else
            {
                output.Add(current);
            }
        }
        return output;
    }
}