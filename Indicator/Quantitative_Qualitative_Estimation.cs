using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using AgenaTrader.API;
using AgenaTrader.Custom;
using AgenaTrader.Plugins;
using AgenaTrader.Helper;

/// Version: 1.0
/// 
//--------------------------------------------------------------
// <auto-generated>
//     This code was generated by AgenaTrader conversion tool.
// </auto-generated>
//--------------------------------------------------------------

// This namespace holds all indicators and is required. Do not change it.
namespace AgenaTrader.UserCode
{
    /// <summary>
    /// Version 1.0 11/04/08
	/// Converted from MQL by Elliott Wave and PrTester.
    /// </summary>
    [Description("Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.")]
    public class QQE : UserIndicator
    {
        #region Variables
        // Wizard generated variables
            private int rSI_Period = 14; // Default setting for RSI_Period
		
			private int Wilders_Period;
			private int StartBar, LastAlertBar;
			private int sF=5;
		
		    private DataSeries TrLevelSlow;
			private DataSeries AtrRsi;
			private DataSeries MaAtrRsi;
			private DataSeries RsiAr;
			private DataSeries RsiMa;
		
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            
			Add(new Plot (new Pen(Color.FromKnownColor(KnownColor.DodgerBlue),2), PlotStyle.Line, "Value1"));
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Gold),2), PlotStyle.Line, "Value2"));
            Plots[1].Pen.DashStyle = DashStyle.Dash;
			
			Add(new Line(Color.Gray, 30, "Lower Line"));
			Add(new Line(Color.Gray, 70, "Upper Line"));
			Add(new Line(Color.Gray, 50, "Mid Line"));
			Lines[0].Pen.DashStyle = DashStyle.Dash;
			Lines[1].Pen.DashStyle = DashStyle.Dash;
			
			
			CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= true;
			
			AtrRsi = new DataSeries(this);
			MaAtrRsi = new DataSeries(this);


			Wilders_Period=rSI_Period * 2 - 1;
			if (Wilders_Period < SF)
				StartBar=SF;
			else
				StartBar=Wilders_Period;
				}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double rsi0, rsi1, dar, tr, dv;
			
			if(CurrentBar <= StartBar)
				return;
			
				
			Value1.Set(EMA(RSI(rSI_Period,3),sF)[0]);
			
				
			
			AtrRsi.Set(Math.Abs(Value1[1] - Value1[0]));
			
			MaAtrRsi.Set(EMA(AtrRsi,Wilders_Period)[0]);
		
			tr = Value2[1];
			rsi1 = Value1[1];

			rsi0 = Value1[0];
			dar = EMA(MaAtrRsi, Wilders_Period)[0] * 4.236;
	
			dv = tr;
			if (rsi0 < tr)
			{
				tr = rsi0 + dar;
				if (rsi1 < dv)
					if (tr > dv)
						tr = dv;
			}
			else if (rsi0 > tr)
			{
				tr = rsi0 - dar;
				if (rsi1 > dv)
					if (tr < dv)
						tr = dv;
			}
			Value2.Set(tr);
		}
		
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Value1
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Value2
        {
            get { return Values[1]; }
        }

        [Description("Period for the RSI")]
        [Category("Parameters")]
        public int RSI_Period
        {
            get { return rSI_Period; }
            set { rSI_Period = Math.Max(1, value); }
        }
		[Description("Smoothing Factor")]
        [Category("Parameters")]
        public int SF
        {
            get { return sF; }
            set { sF = Math.Max(1, value); }
        }
		
		
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
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(System.Int32 rSI_Period, System.Int32 sF)
        {
			return QQE(Input, rSI_Period, sF);
		}

		/// <summary>
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(IDataSeries input, System.Int32 rSI_Period, System.Int32 sF)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<QQE>(input, i => i.RSI_Period == rSI_Period && i.SF == sF);

			if (indicator != null)
				return indicator;

			indicator = new QQE
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							RSI_Period = rSI_Period,
							SF = sF
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
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(System.Int32 rSI_Period, System.Int32 sF)
		{
			return LeadIndicator.QQE(Input, rSI_Period, sF);
		}

		/// <summary>
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(IDataSeries input, System.Int32 rSI_Period, System.Int32 sF)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.QQE(input, rSI_Period, sF);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(System.Int32 rSI_Period, System.Int32 sF)
		{
			return LeadIndicator.QQE(Input, rSI_Period, sF);
		}

		/// <summary>
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(IDataSeries input, System.Int32 rSI_Period, System.Int32 sF)
		{
			return LeadIndicator.QQE(input, rSI_Period, sF);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(System.Int32 rSI_Period, System.Int32 sF)
		{
			return LeadIndicator.QQE(Input, rSI_Period, sF);
		}

		/// <summary>
		/// Qualitative Quantitative Estimation. QQE is a combination moving average RSI + ATR.
		/// </summary>
		public QQE QQE(IDataSeries input, System.Int32 rSI_Period, System.Int32 sF)
		{
			return LeadIndicator.QQE(input, rSI_Period, sF);
		}
	}

	#endregion

}

#endregion
