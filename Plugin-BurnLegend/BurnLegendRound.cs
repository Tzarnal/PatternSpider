using System.Collections.Generic;
using System.Linq;

namespace Plugin_BurnLegend
{
    class BurnLegendRound
    {
        public List<BurnLegendAction> Actions;

        public BurnLegendRound()
        {
            Actions = new List<BurnLegendAction>();
        }

        public void AddAction(string userName, string actionDescription)
        {
            Actions.Add(new BurnLegendAction{ActionDescription = actionDescription, UserName = userName});
        }

        public void ClearActionsBy(string userName)
        {
            Actions.RemoveAll(a => a.UserName == userName);
        }

        public List<string> Reveal()
        {
            var revealedActions = new List<string>();

            foreach (var action in Actions)
            {
                revealedActions.Add(string.Format("{0} played: {1}",action.UserName,action.ActionDescription));
            }

            return revealedActions;
        }

        public string Status()
        {
            var statusline = "Status of round: ";

            if (!Actions.Any())
            {
                return "No action yet in this round.";
            }

            var names = Actions.GroupBy(a => a.UserName);

            foreach (var name in names)
            {
                statusline += string.Format("{0}[{1}] ", name.Key, name.Count());
            }

            return statusline;
        }
    }
}
