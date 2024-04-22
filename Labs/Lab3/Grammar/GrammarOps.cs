using Grammar;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
	public class GrammarOps
	{
		public GrammarOps(IGrammar g)
		{
			this.g = g;
			compute_empty();
		}

		public ISet<Nonterminal> EmptyNonterminals { get; } = new HashSet<Nonterminal>();
		private void compute_empty()
		{
			///TODO: Add your code here...
			foreach(var rule in g.Rules)
			{
				if (rule.RHS.Count == 0) EmptyNonterminals.Add(rule.LHS);
			}

			int count = 0;
			do
			{
				foreach (var rule in g.Rules)
				{
					count = EmptyNonterminals.Count;
					if (rule.RHS.All(x => x is Nonterminal && EmptyNonterminals.Contains(x)))
						EmptyNonterminals.Add(rule.LHS);
				}
			} while (count != EmptyNonterminals.Count);

		}

		private IGrammar g;
	}
}
