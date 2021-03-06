using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using AgenaTrader.API;
using AgenaTrader.Custom;
using AgenaTrader.Plugins;
using AgenaTrader.Helper;

/// <summary>
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.")]
	public class QuickAdd : UserIndicator
	{
		#region Variables

		    private string _name_of_list = String.Empty;
            private string _shortcut_list = String.Empty;
            private IInstrumentsList _list = null;
            private RectangleF _rect;
            //private Pen _pen = Pens.Black;
            private Brush _brush = Brushes.Gray;

		#endregion

        protected override void Initialize()
        {
            //Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            Overlay = true;
        }


        protected override void OnStartUp()
        {
           
            // Add event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseDown += OnChartPanelMouseDown;

            if (this.Instrument != null)
            {
                if (!String.IsNullOrEmpty(Name_of_list))
                {

                    this.Root.Core.InstrumentManager.GetInstrumentLists();
                    _list = this.Root.Core.InstrumentManager.GetInstrumentsListStatic(this.Name_of_list);
                    //if (_list == null)
                    //{
                    //    _list = this.Root.Core.InstrumentManager.GetInstrumentsListDynamic(this.Name_of_list);
                    //}
                    if (_list == null || _list.Count == 0)
                    {
                        Log(this.DisplayName + ": The list " + this.Name_of_list + " does not exist.", InfoLogLevel.Warning);
                    }
                }
                else
                {
                    Log(this.DisplayName + ": You need to specify a name for the list.", InfoLogLevel.Warning);
                }
            }

        }


		protected override void OnBarUpdate()
		{
			//MyPlot1.Set(Input[0]);

            if (this.IsCurrentBarLast && _list != null && _list.Count > 0)
            {
                if (_list.Contains((Instrument)this.Instrument))
                {
                    //_pen = Pens.Red;
                    _brush = Brushes.Green;
                }
                else {
                    //_pen = Pens.Black;
                    _brush = Brushes.Gray;
                }
             }

		}

        protected override void OnTermination()
        {
            // Remove event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseDown -= OnChartPanelMouseDown;
        }


        public override string DisplayName
        {
            get
            {
                return "QuickAdd";
            }
        }


        public override string ToString()
        {
            return "QuickAdd";
        }


        #region Events



            public override void Plot(Graphics g, Rectangle r, double min, double max)
            {
                if (Bars == null || ChartControl == null) return;

                //Only draw button if parameters are available.
                if (this.Instrument != null && _list != null && _list.Count > 0)
                {
                    using (Font font1 = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point))
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        if (String.IsNullOrEmpty(Shortcut_list))
                        {
                            if (this.Name_of_list.Count() >= 5)
                            {
                                this.Shortcut_list = this.Name_of_list.Substring(0, 5);
                            }
                            else
                            {
                                this.Shortcut_list = this.Name_of_list;
                            }
                        }

                        Brush tempbrush = new SolidBrush(GlobalUtilities.AdjustOpacity(((SolidBrush)_brush).Color, 0.5F));

                        _rect = new RectangleF(r.Width - 100, 10, 86, 27);
                        g.FillRectangle(tempbrush, _rect);
                        g.DrawString(Shortcut_list, font1, Brushes.White, _rect, stringFormat);
                        //g.DrawRectangle(_pen, Rectangle.Round(_rect));
                    }
                }
            }


            private void OnChartPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                //Print("X = {0}, Y = {1}", e.X, e.Y);
                //Print("X = {0}, Y = {1}", ChartControl.GetDateTimeByX(e.X), ChartControl.GetPriceByY(e.Y));

                Point cursorPos = new Point(e.X, e.Y);
                if (_rect.Contains(cursorPos))
                {
                    if (!_list.Contains((Instrument)this.Instrument))
                    {
                        this.Root.Core.InstrumentManager.AddInstrument2List(this.Instrument, this.Name_of_list);
                    }
                    else
                    {
                        this.Root.Core.InstrumentManager.RemoveInstrumentFromList(this.Name_of_list, this.Instrument);
                    }
                }
                else
                {
                  //nothing to do
                }

                this.OnBarUpdate();
               
            }

        #endregion



        #region Properties

        #region Output

        //[Browsable(false)]
        //    [XmlIgnore()]
        //    public DataSeries MyPlot1
        //    {
        //        get { return Values[0]; }
        //    }

            #endregion

            #region Input


            [Description("The name of the static list to which you would like to add the instruments.")]
            //[Category("Values")]
            [DisplayName("Static list")]
            public string Name_of_list
            {
                get { return _name_of_list; }
                set { _name_of_list = value; }
            }

            [Description("Shortcut is used to dsiplay the name of the static list in a button.")]
            //[Category("Values")]
            [DisplayName("Shortcut for the list")]
            public string Shortcut_list
            {
                get { return _shortcut_list; }
                set { _shortcut_list = value; }
            }

            #endregion

  

		#endregion

        
    }
}

#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd()
        {
			return QuickAdd(Input);
		}

		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<QuickAdd>(input);

			if (indicator != null)
				return indicator;

			indicator = new QuickAdd
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd()
		{
			return LeadIndicator.QuickAdd(Input);
		}

		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.QuickAdd(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd()
		{
			return LeadIndicator.QuickAdd(Input);
		}

		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			return LeadIndicator.QuickAdd(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd()
		{
			return LeadIndicator.QuickAdd(Input);
		}

		/// <summary>
		/// Adds an instrument to a static list (e.g. watchlist) by clicking on a button in the chart.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			return LeadIndicator.QuickAdd(input);
		}
	}

	#endregion

}

#endregion


