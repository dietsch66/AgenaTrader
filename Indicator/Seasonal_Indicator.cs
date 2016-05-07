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
using System.Runtime.CompilerServices;


/// <summary>
/// Version: in progress
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// The initial version of this strategy was inspired by this link: http://emini-watch.com/stock-market-seasonal-trades/5701/
/// -------------------------------------------------------------------------
/// todo tax time, 4th of july, september effect, Thanksgiving
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
  
    /// <summary>
    /// Differend types of seasonal indictors.
    /// </summary>
    public enum SeasonalType
    {
        SellInMay = 1,
        SantaClausRally = 2
    }

    [Description("Show seasonal trends")]
	public class Seasonal_Indicator : UserIndicator
	{
      

        #region Variables

        //input
        private SeasonalType _seasonal = SeasonalType.SellInMay;

        //output


        //internal
        private RectangleF _rect;

        private IEnumerable<IBar> _list_sellinmayandgoaway_buy = null;
        private IEnumerable<IBar> _list_sellinmayandgoaway_sell = null;
        private IBar _last_start_sellinmayandgoway = null;
        private IBar _last_end_sellinmayandgoway = null;

        private IEnumerable<IBar> _list_santaclausrally_buy = null;
        private IBar _last_start_santaclausrally = null;

        //Save data into hashset for a performance boost on the contains method
        private HashSet<DateTime> hashset = null;

        #endregion


		protected override void Initialize()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
		}

        protected override void InitRequirements()
        {
            //Print("InitRequirements");

        }
   

        protected override void OnStartUp()
        {
            //Print("OnStartUp");

            //DateTime start = DateTime.Now;

            switch (SeasonalType)
            {
                case SeasonalType.SellInMay:
                    _list_sellinmayandgoaway_buy = Bars.Where(x => x.Time.Month <= 4 || x.Time.Month >= 10);
                    _list_sellinmayandgoaway_sell = Bars.Except(_list_sellinmayandgoaway_buy);
                    hashset = new HashSet<DateTime>(_list_sellinmayandgoaway_buy.Select(x => x.Time));
                    break;
                case SeasonalType.SantaClausRally:
                    _list_santaclausrally_buy = from b in Bars
                                               where (b.Time.Month == 12 && b.Time.Day >= 15) || (b.Time.Month == 1 && b.Time.Day <= 9)
                                               select b;
                    hashset = new HashSet<DateTime>(_list_santaclausrally_buy.Select(x => x.Time));
                    break;
                default:
                    break;
            }


           //DateTime stop = DateTime.Now;
           //TimeSpan dif = stop - start;
           //Print(dif.TotalMilliseconds);
        }

        DateTime start_onbarupdate = DateTime.Now;
        DateTime stop_onbarudate = DateTime.Now;

		protected override void OnBarUpdate()
		{
            if (CurrentBar == 0)
            {
                start_onbarupdate = DateTime.Now;
            }

			//MyPlot1.Set(Input[0]);

            ////Because of performance just draw on the last bar update.
            //if (this.IsCurrentBarLast)
            //{
            //    DrawAreaRectangle(list_sellinmayandgoaway_buy_new, Color.Green);
            //    DrawAreaRectangle(list_sellinmayandgoaway_sell_new, Color.Red);
            //}

            switch (SeasonalType)
            {
                case SeasonalType.SellInMay:
                    this.calculate_Sell_in_May();
                    break;
                case SeasonalType.SantaClausRally:
                    this.calculate_Santa_Claus_Rally();
                    break;
                default:
                    break;
            }


            if (this.IsCurrentBarLast)
            {
                DateTime stop_onbarudate = DateTime.Now;
                TimeSpan dif = stop_onbarudate - start_onbarupdate;
                Print(dif.TotalMilliseconds);
            }
        
            
		}

        /// <summary>
        /// Draws a rectangle in the chart to visualize the seasonality.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="col"></param>
        private void DrawAreaRectangle(IEnumerable<IBar> list, Color color)
        {
               double low = list.Min(x => x.Low);
                double high = list.Max(x => x.High);

                double difference = list.Last().Close - list.First().Open;

                DrawRectangle("sellinmayRect_buy" + list.First().Time.ToString(), true, list.First().Time, high, list.Last().Time, low, color, color, 70);
                DrawText("sellinmayString_buy" + list.First().Time.ToString(), true, Math.Round((difference), 2).ToString(), list.First().Time, high, 7, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, color, 100);
        }

        protected override void OnTermination()
        {
            //// Remove event listener
            //if (ChartControl != null)
            //    ChartControl.ChartPanelMouseDown -= OnChartPanelMouseDown;
        }

        /// <summary>
        /// Calculate the seasonal indicator for "Santa Claus Rally".
        /// </summary>
        private void calculate_Santa_Claus_Rally()
        {
            //if (list_santaclausrally_buy.Select(x => x.Time).Contains(Bars[0].Time))
            if (hashset.Contains(Bars[0].Time))
            {
                if (this._last_start_santaclausrally == null)
                {
                    this._last_start_santaclausrally = Bars[0];
                }
            }
            else
            {
                if (this._last_start_santaclausrally != null)
                {
                    DrawAreaRectangle(this._list_santaclausrally_buy.Where(x => x.Time >= this._last_start_santaclausrally.Time).Where(x => x.Time <= Bars[0].Time), Color.Green);

                    this._last_start_santaclausrally = null;
                }
            }
        }


        /// <summary>
        /// Calculate the seasonal indicator for "Sell in May".
        /// </summary>
        private void calculate_Sell_in_May() {
            //if (list_sellinmayandgoaway_buy.Select(x=>x.Time).Contains(Bars[0].Time))
            if (hashset.Contains(Bars[0].Time))
            {
                if (_last_start_sellinmayandgoway == null)
                {
                    this._last_start_sellinmayandgoway = Bars[0];
                }

                if (_last_end_sellinmayandgoway != null)
                {
                    DrawAreaRectangle(_list_sellinmayandgoaway_sell.Where(x => x.Time >= _last_end_sellinmayandgoway.Time).Where(x => x.Time <= Bars[0].Time), Color.Red);

                    _last_end_sellinmayandgoway = null;
                }

            }
            else
            {
                if (_last_end_sellinmayandgoway == null)
                {
                    this._last_end_sellinmayandgoway = Bars[0];
                }

                if (_last_start_sellinmayandgoway != null)
                {
                    DrawAreaRectangle(_list_sellinmayandgoaway_buy.Where(x => x.Time >= _last_start_sellinmayandgoway.Time).Where(x => x.Time <= Bars[0].Time), Color.Green);

                    _last_start_sellinmayandgoway = null;
                }
            }
        }



        public override string ToString()
        {
            return "Seasonal";
        }

        public override string DisplayName
        {
            get
            {
                return "Seasonal";
            }
        }




		#region Properties

        #region Input 
        
            /// <summary>
            /// </summary>
            [Description("Seasonal Type")]
            [Category("Parameters")]
            [DisplayName("Seasonal Type")]
            public SeasonalType SeasonalType
            {
                get { return _seasonal; }
                set { _seasonal = value; }
            }

	    #endregion

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot1
        //{
        //    get { return Values[0]; }
        //}


        

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
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
        {
			return Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Seasonal_Indicator>(input, i => i.SeasonalType == seasonalType);

			if (indicator != null)
				return indicator;

			indicator = new Seasonal_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							SeasonalType = seasonalType
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
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Seasonal_Indicator(input, seasonalType);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(input, seasonalType);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(input, seasonalType);
		}
	}

	#endregion

}

#endregion
