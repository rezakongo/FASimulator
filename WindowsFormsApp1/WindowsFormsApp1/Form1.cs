using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        class draw
        {
           static public void nfagraph(List<State> list)
           {
                System.Windows.Forms.Form form = new System.Windows.Forms.Form();
                Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
                for(int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list[i].transitions.Count; j++)
                    {
                        graph.AddEdge(list[i].Name, list[i].transitions[j].input, list[i].transitions[j].Goto.Name);
                    }
                    if (list[i].isFinal == true)
                    {
                        Microsoft.Msagl.Drawing.Node c = graph.FindNode(list[i].Name);
                        c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                    }
                }
                viewer.Graph = graph;
                form.SuspendLayout();
                viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                form.Controls.Add(viewer);
                form.ResumeLayout();
                form.ShowDialog();
           }
            static public void dfagraph(List<DFAState> list)
            {
                System.Windows.Forms.Form form = new System.Windows.Forms.Form();
                Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list[i].transitions.Count; j+=2)
                    {
                        if (list[i].transitions[j].input != "")
                        {
                            graph.AddEdge(list[i].name, list[i].transitions[j].input, list[i].transitions[j].nextState.name);
                        }
                        if(list[i].isFinalState == true)
                        {
                            Microsoft.Msagl.Drawing.Node c = graph.FindNode(list[i].name);
                            c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                        }
                    }
                }
                viewer.Graph = graph;
                form.SuspendLayout();
                viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                form.Controls.Add(viewer);
                form.ResumeLayout();
                form.ShowDialog();
            }
        }
        class nfaTransition
        {
            public string input;
            public State Goto;

            public nfaTransition(string inp, State gotos)
            {
                input = inp;
                this.Goto = gotos;
            }
        }
        class State
        {
            public string Name;
            public List<nfaTransition> transitions;
            public bool isFinal;
            public State(string Name)
            {
                this.Name = Name;
                transitions = new List<nfaTransition>();
                isFinal = false;
            }
        }
        class NFA
        {
            public State initialState;
            public List<State> States;
            public List<string> alphabets;
            public NFA originalNFA;
            public NFA(List<string> alphas)
            {
                States = new List<State>();
                alphabets = alphas;
                alphabets.Add("");
            }
            //This Method is a part of Regex
            public void getReadyforRegex()
            {
                for (int i = 0; i < States.Count; i++)
                {
                    for (int j = 0; j < States[i].transitions.Count - 1; j++)
                    {
                        for (int k = j + 1; k < States[i].transitions.Count; k++)
                        {
                            if (States[i].transitions[j].Goto.Name == States[i].transitions[k].Goto.Name)
                            {
                                if (States[i].transitions[j].input.Length > 1)
                                {
                                    if ((States[i].transitions[j].input[0].ToString() == "(") && (States[i].transitions[j].input[States[i].transitions[j].input.Length - 1].ToString() == ")"))
                                    {
                                        String sw = "";
                                        for (int z = 1; z < States[i].transitions[j].input.Length - 1; z++)
                                        {
                                            sw = sw + States[i].transitions[j].input[z].ToString();
                                        }
                                        States[i].transitions[j].input = sw;
                                    }
                                }
                                if (States[i].transitions[j].input == "")
                                {
                                    States[i].transitions[j].input += (States[i].transitions[k].input);
                                }
                                else
                                {
                                    States[i].transitions[j].input += ("+" + States[i].transitions[k].input);
                                }
                                States[i].transitions.RemoveAt(k);
                                k--;
                            }
                        }
                        if (States[i].transitions[j].input.Length > 0)
                        {
                            if (States[i].transitions[j].input.Split('+').Length > 1)
                            {
                                States[i].transitions[j].input = ("(" + States[i].transitions[j].input + ")");
                            }
                        }
                    }
                }
            }
            //This Method create Regex for NFA 
            public string getRegularExpression()
            {
                string ans = "";
                getReadyforRegex();
                while (States.Count != 2)
                {
                    
                    List<State> inState = new List<State>();
                    string loop = "";

                    foreach (nfaTransition transition in States[2].transitions)
                    {
                        if (transition.Goto.Name == States[2].Name)
                        {
                            if (transition.input.Length > 0)
                            {
                                if (transition.input[0] == '(' && transition.input[transition.input.Length - 1] == ')')
                                {
                                    loop = (transition.input + "*");
                                }
                                else
                                {
                                    if (transition.input.Length > 1)
                                    {
                                        loop = ("(" + transition.input + ")*");
                                    }
                                    else
                                    {
                                        loop = (transition.input + "*");
                                    }
                                }
                            }
                            break;
                        }
                    }

                    for (int i = 0; i < States.Count; i++)
                    {
                        if (i != 2)
                        {
                            foreach (nfaTransition transition in States[i].transitions)
                            {
                                if (transition.Goto.Name == States[2].Name)
                                {
                                    inState.Add(States[i]);
                                    break;
                                }
                            }
                        }
                    }

                    foreach (State state in inState)
                    {
                        foreach (nfaTransition inTransition in state.transitions)
                        {
                            if (inTransition.Goto.Name == States[2].Name)
                            {
                                int Counter = States[2].transitions.Count;
                                for (int i = 0; i < Counter; i++)
                                {
                                    state.transitions.Add(new nfaTransition(((inTransition.input) + (loop) + (States[2].transitions[i].input)), States[2].transitions[i].Goto));
                                }
                                break;
                            }
                        }
                    }
                    States.RemoveAt(2);
                    getReadyforRegex();
                }
                foreach (nfaTransition transition in States[0].transitions)
                {
                    if (transition.Goto.Name == States[1].Name)
                    {
                        ans = transition.input;
                        break;
                    }
                }
                return ans;
            }
            //This Method Checks a String Acceptation
            public bool isAcceptByNFA(string str, State Current, int start, int end)
            {
                if (start > end)
                {
                    if (Current.isFinal)
                    {
                        return true;
                    }
                    else
                    {
                        bool ans = false;
                        foreach (nfaTransition transition in Current.transitions)
                        {
                            if (transition.input == "")
                            {
                                ans = (ans || isAcceptByNFA(str, transition.Goto, start, end));
                            }
                        }
                        return ans;
                    }
                }
                else
                {
                    if (Current.transitions.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        bool ans = false;
                        foreach (nfaTransition transition in Current.transitions)
                        {
                            if (transition.input == str[start].ToString())
                            {
                                ans = (ans || isAcceptByNFA(str, transition.Goto, start + 1, end));
                            }
                            if (transition.input == "")
                            {
                                ans = (ans || isAcceptByNFA(str, transition.Goto, start, end));
                            }
                        }
                        return ans;
                    }
                }
            }
            //This method call NtoD
            public DFA NFAtoDFA()
            {
                DFA equalDFA = new DFA(initialState, alphabets);
                NtoD(equalDFA.initialState, equalDFA);
                for(int i = 0; i < equalDFA.dFAStates.Count; i++)
                {
                    equalDFA.dFAStates[i].name = "g" + i;
                }
                return equalDFA;
            }

            //This method Convetrs The NFA to a DFA
            public void NtoD(DFAState state, DFA dFA)
            {
                for(int i = 0; i < state.states.Count; i++)
                {
                    for(int j = 0; j < state.states[i].transitions.Count; j++)
                    {
                        if (state.states[i].transitions[j].input == "")
                        {
                            if (!state.isExist(state.states[i].transitions[j].Goto))
                            {
                                state.states.Add(state.states[i].transitions[j].Goto);
                            }
                        }
                    }
                }
                
                
                foreach (string str in alphabets)
                {
                    if (str != "")
                    {
                        bool backtoT = true;
                        DFAState newDfAState = new DFAState(alphabets);
                        foreach (State s in state.states)
                        {
                            foreach (nfaTransition nt in s.transitions)
                            {
                                if (nt.input == str)
                                {
                                    backtoT = false;
                                    if (!newDfAState.isExist(nt.Goto))
                                    {
                                        newDfAState.add(nt.Goto);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < newDfAState.states.Count; i++)
                        {
                            for (int j = 0; j < newDfAState.states[i].transitions.Count; j++)
                            {
                                if (newDfAState.states[i].transitions[j].input == "")
                                {
                                    if (!newDfAState.isExist(newDfAState.states[i].transitions[j].Goto))
                                    {
                                        newDfAState.states.Add(newDfAState.states[i].transitions[j].Goto);
                                    }
                                }
                            }
                        }
                        if (backtoT)
                        {
                            state.transitions.Add(new DFATransition(dFA.tState, str));
                        }
                        else
                        {
                            if (dFA.isExist(newDfAState) == null)
                            {
                                
                                dFA.dFAStates.Add(newDfAState);
                                state.transitions.Add(new DFATransition(newDfAState, str));
                                NtoD(newDfAState, dFA);
                            }
                            else
                            {
                                state.transitions.Add(new DFATransition(dFA.isExist(newDfAState), str));
                            }
                        }
                    }
                }
            }
        }
        class DFATransition
        {
            public DFAState nextState;
            public string input;

            public DFATransition(DFAState ds, string inp)
            {
                nextState = ds;
                input = inp;
            }
        }
        class DFAState
        {
            public string name;
            public List<State> states;
            public bool isFinalState;
            public List<string> alphabets;
            public bool isChecked;
            public List<DFATransition> transitions;

            public DFAState(List<string> alphas)
            {
                isChecked = false;
                alphabets = alphas;
                isFinalState = false;
                states = new List<State>();
                transitions = new List<DFATransition>();
            }
            //this method add a State to DFA State
            public void add(State state)
            {
                states.Add(state);
                if (state.isFinal)
                {
                    isFinalState = true;
                }
            }
            //This method checks that a state is exist in the list or not
            public bool isExist(State state)
            {
                bool ans = false;
                foreach (State s in states)
                {
                    if (s == state)
                    {
                        ans = true;
                        break;
                    }
                }
                return ans;
            }
            //This Method Compare 2 DFAStates
            public bool isEqualto(DFAState otherState)
            {
                if (states.Count == otherState.states.Count)
                {
                    bool ans = true;
                    foreach (State x in otherState.states)
                    {
                        ans = false;
                        foreach (State y in states)
                        {
                            if (x.Name == y.Name)
                            {
                                ans = true;
                                break;
                            }
                        }

                        if (!ans)
                        {
                            break;
                        }
                    }
                    return ans;
                }
                else
                {
                    return false;
                }
            }
        }
        class DFA
        {
            public DFAState initialState;
            public List<DFAState> dFAStates;
            public DFAState tState;

            public DFA(State initState, List<string> alphabets)
            {
                dFAStates = new List<DFAState>();
                initialState = new DFAState(alphabets);
                initialState.states.Add(initState);
                dFAStates.Add(initialState);
                tState = new DFAState(alphabets);
                State t = new State("gt");
                tState.add(t);
                foreach (string alpha in alphabets)
                {
                    tState.transitions.Add(new DFATransition(tState,alpha));
                }
                tState.isChecked = true;
                dFAStates.Add(tState);
            }
            //This Method Checks a String Acceptation by the DFA
            public bool isAcceptByDFA(string str,DFAState Current, int start, int end)
            {
                if (start > end)
                {
                    if (Current.isFinalState)
                    {
                        return true;
                    }
                    else
                    {
                        bool ans = false;
                        foreach (DFATransition transition in Current.transitions)
                        {
                            if (transition.input == "")
                            {
                                ans = (ans || isAcceptByDFA(str, transition.nextState, start, end));
                            }
                        }
                        return ans;
                    }
                }
                else
                {
                    if (Current.transitions.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        bool ans = false;
                        foreach (DFATransition transition in Current.transitions)
                        {
                            if (transition.input == str[start].ToString())
                            {
                                ans = (ans || isAcceptByDFA(str, transition.nextState, start + 1, end));
                            }
                            if (transition.input == "")
                            {
                                ans = (ans || isAcceptByDFA(str, transition.nextState, start, end));
                            }
                        }
                        return ans;
                    }
                }
            }
            //This Method checks if a DFAState Exists in the DFA or Not
            public DFAState isExist(DFAState ds)
            {
                DFAState ans = null;
                foreach (DFAState dFAState in dFAStates)
                {
                    if (dFAState.isEqualto(ds))
                    {
                        ans = dFAState;
                        break;
                    }
                }
                return ans;
            }
            //This Method Minimize a Complicated DFA
            public void DfaMinimization()
            {
                int a = this.dFAStates.Count;
                int b = dFAStates[0].alphabets.Count;
                string[,] array = new string[a, b];
                for(int i =0; i < a; i++)
                {
                    array[i, 0] = dFAStates[i].name;
                }
                for(int i = 0; i < a; i++)
                {
                    for(int j = 1; j < a; j++)
                    {
                        array[i, j] = dFAStates[i].transitions[j].nextState.ToString();
                    }
                }

                for (int i = 1; i < a; i++)
                {
                    if (array[i, 0] != "-")
                    {
                        for (int j = 0; j < a; j++)
                        {
                            if (j != i )
                            {
                                bool breaka = false;
                                for(int z = 1; z < b; z++)
                                {
                                    if (array[i,0] == array[j,z])
                                    {
                                        breaka = true;
                                        break;
                                    }
                                }
                                if(breaka == true)
                                {
                                    break;
                                }
                            }
                            if (j == a - 1)
                            {
                                array[i, 0] = "-";
                            }
                        }
                    }
                }


                for (int i = 0; i < a; i++)
                {
                    if (array[i, 0] == "-")
                    {
                        break;
                    }
                    for (int j = i + 1; j < a; j++)
                    {
                        if (array[j, 0] != "-")
                        {
                            bool isok = true;
                            for (int z = i + 1; z < b; z++)
                            {
                                if (array[i, z] != array[j, z])
                                {
                                    isok = false;
                                    break;
                                }
                            }
                            if(isok == true)
                            {
                                array[j, 0] = array[i, 0];
                            }
                        }
                    }
                }


                for (int i = 0; i < a; i++)
                {
                    if (array[i, 0] == "-")
                    {
                        break;
                    }
                    for (int j = i + 1; j < a; j++)
                    {
                        if (array[j, 0] != "-")
                        {
                            for(int z = 1; z < b; z++)
                            {
                                if (array[i, 0] == array[j, z] && array[i, z] == array[j, 0])
                                {
                                    array[j, 0] = array[i, 0];
                                }
                            }
                        }
                    }
                }
            }
        }
        NFA nfa;
        public Form1()
        {
            InitializeComponent();
            List<string> l = new List<string>();
            nfa = new NFA(l);
            nfa.originalNFA = new NFA(l);
            nfa.originalNFA.initialState = new State("Qi");
            nfa.originalNFA.States.Add(nfa.originalNFA.initialState);
            nfa.originalNFA.States.Add(new State("QF"));
            nfa.originalNFA.States[1].isFinal = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            draw.nfagraph(nfa.States);
        }

        private void button2_Click(object sender, EventArgs e)
        {
           for (int i =0; i< nfa.States.Count; i++)
           {
                if(nfa.States[i].Name == textBox1.Text)
                {
                    for (int j = 0; j < nfa.States.Count; j++)
                    {
                        if(nfa.States[j].Name == textBox3.Text)
                        {
                            if (nfa.alphabets.Contains(textBox2.Text))
                            {
                                nfa.States[i].transitions.Add(new nfaTransition(textBox2.Text, nfa.States[j]));
                                nfa.originalNFA.States[i + 2].transitions.Add(new nfaTransition(textBox2.Text, nfa.originalNFA.States[j + 2]));
                                listBox1.Items.Add(nfa.States[i].Name + textBox2.Text + "->" + nfa.States[j].Name);
                            }
                        }
                    }
                }
           }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                nfa.alphabets.Add(textBox4.Text);
                nfa.originalNFA.alphabets.Add(textBox4.Text);
                listBox2.Items.Add(textBox4.Text);
            }
            textBox4.Text = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox5.Text != "")
            {
                State s = new State(textBox5.Text);
                State s1 = new State(textBox5.Text);
                if (checkBox1.Checked == true)
                {
                    s.isFinal = true;
                    s1.transitions.Add(new nfaTransition("", nfa.originalNFA.States[1]));
                }
                nfa.States.Add(s);
                nfa.originalNFA.States.Add(s1);
                nfa.initialState = nfa.States[0];
                if (checkBox1.Checked == true)
                {
                    listBox3.Items.Add(textBox5.Text + "(f)");
                }
                else
                {
                    listBox3.Items.Add(textBox5.Text);
                }
            }
            nfa.originalNFA.initialState.transitions.Add(new nfaTransition("", nfa.originalNFA.States[2]));
            checkBox1.Checked = false;
            textBox5.Text = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox7.Text = nfa.isAcceptByNFA(textBox6.Text, nfa.initialState, 0, textBox6.Text.Length - 1).ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DFA dfa = nfa.NFAtoDFA();
            draw.dfagraph(dfa.dFAStates);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox8.Text = nfa.originalNFA.getRegularExpression();
        }
    }
}
