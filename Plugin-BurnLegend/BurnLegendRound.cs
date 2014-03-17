using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_BurnLegend
{
    class BurnLegendRound
    {
        public List<Action> Actions;

        public BurnLegendRound()
        {
            Actions = new List<Action>();
        }

        public void AddAction(string userName, string actionDescription)
        {
            Actions.Add(new Action{ActionDescription = actionDescription, UserName = userName});
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
    }
}
