using CommonLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PlaneXmlWriter
{
    public class XmlModel : ObservableObject
    {
        // **A** the xml paths 
        public const string XPATH_REF_CHORD = "/CRRCSim_airplane/aero/ref/@chord";
        public const string XPATH_REF_SPAN = "/CRRCSim_airplane/aero/ref/@span";
        public const string XPATH_REF_AREA = "/CRRCSim_airplane/aero/ref/@area";
        public const string XPATH_REF_SPEED = "/CRRCSim_airplane/aero/ref/@speed";

        public const string XPATH_MISC_ALPHA_0 = "/CRRCSim_airplane/aero/misc/@Alpha_0";
        public const string XPATH_MISC_SPAN_EFF = "/CRRCSim_airplane/aero/misc/@span_eff";

        public const string XPATH_M_CM_0 = "/CRRCSim_airplane/aero/m/@Cm_0";
        public const string XPATH_M_CM_A = "/CRRCSim_airplane/aero/m/@Cm_a";
        public const string XPATH_M_CM_Q = "/CRRCSim_airplane/aero/m/@Cm_q";
        public const string XPATH_M_CM_DE = "/CRRCSim_airplane/aero/m/@Cm_de";

        public const string XPATH_LIFT_CL_0 = "/CRRCSim_airplane/aero/lift/@CL_0";
        public const string XPATH_LIFT_CL_MAX = "/CRRCSim_airplane/aero/lift/@CL_max";
        public const string XPATH_LIFT_CL_MIN = "/CRRCSim_airplane/aero/lift/@CL_min";
        public const string XPATH_LIFT_CL_A = "/CRRCSim_airplane/aero/lift/@CL_a";
        public const string XPATH_LIFT_CL_Q = "/CRRCSim_airplane/aero/lift/@CL_q";
        public const string XPATH_LIFT_CL_DE = "/CRRCSim_airplane/aero/lift/@CL_de";
        public const string XPATH_LIFT_CL_DROP = "/CRRCSim_airplane/aero/lift/@CL_drop";
        public const string XPATH_LIFT_CL_CD0 = "/CRRCSim_airplane/aero/lift/@CL_CD0";

        public const string XPATH_DRAG_CD_PROF = "/CRRCSim_airplane/aero/drag/@CD_prof";
        public const string XPATH_DRAG_UEXP_CD = "/CRRCSim_airplane/aero/drag/@Uexp_CD";
        public const string XPATH_DRAG_CD_STALL = "/CRRCSim_airplane/aero/drag/@CD_stall";
        public const string XPATH_DRAG_CD_CLSQ = "/CRRCSim_airplane/aero/drag/@CD_CLsq";
        public const string XPATH_DRAG_CD_AISQ = "/CRRCSim_airplane/aero/drag/@CD_AIsq";
        public const string XPATH_DRAG_CD_ELSQ = "/CRRCSim_airplane/aero/drag/@CD_ELsq";

        public const string XPATH_Y_CY_B = "/CRRCSim_airplane/aero/Y/@CY_b";
        public const string XPATH_Y_CY_P = "/CRRCSim_airplane/aero/Y/@CY_p";
        public const string XPATH_Y_CY_R = "/CRRCSim_airplane/aero/Y/@CY_r";
        public const string XPATH_Y_CY_DR = "/CRRCSim_airplane/aero/Y/@CY_dr";
        public const string XPATH_Y_CY_DA = "/CRRCSim_airplane/aero/Y/@CY_da";

        public const string XPATH_L_CL_B = "/CRRCSim_airplane/aero/l/@Cl_b";
        public const string XPATH_L_CL_P = "/CRRCSim_airplane/aero/l/@Cl_p";
        public const string XPATH_L_CL_R = "/CRRCSim_airplane/aero/l/@Cl_r";
        public const string XPATH_L_CL_DR = "/CRRCSim_airplane/aero/l/@Cl_dr";
        public const string XPATH_L_CL_DA = "/CRRCSim_airplane/aero/l/@Cl_da";

        public const string XPATH_N_CN_B = "/CRRCSim_airplane/aero/n/@Cn_b";
        public const string XPATH_N_CN_P = "/CRRCSim_airplane/aero/n/@Cn_p";
        public const string XPATH_N_CN_R = "/CRRCSim_airplane/aero/n/@Cn_r";
        public const string XPATH_N_CN_DR = "/CRRCSim_airplane/aero/n/@Cn_dr";
        public const string XPATH_N_CN_DA = "/CRRCSim_airplane/aero/n/@Cn_da";

        public const string XPATH_FLAPS_DRAG = "/CRRCSim_airplane/aero/flaps/@drag";
        public const string XPATH_FLAPS_LIFT = "/CRRCSim_airplane/aero/flaps/@lift";
        public const string XPATH_FLAPS_MOMENT = "/CRRCSim_airplane/aero/flaps/@moment";
        public const string XPATH_FLAPS_EFF_RATIO = "/CRRCSim_airplane/aero/flaps/@eff_ratio";

        public const string XPATH_MASS_INERTIA_MASS = "/CRRCSim_airplane/mass_inertia/@mass";
        public const string XPATH_MASS_INERTIA_I_XX = "/CRRCSim_airplane/mass_inertia/@I_xx";
        public const string XPATH_MASS_INERTIA_I_YY = "/CRRCSim_airplane/mass_inertia/@I_yy";
        public const string XPATH_MASS_INERTIA_I_ZZ = "/CRRCSim_airplane/mass_inertia/@I_zz";
        public const string XPATH_MASS_INERTIA_I_XZ = "/CRRCSim_airplane/mass_inertia/@I_xz";

        public const string XPATH_POWER_AUTOMAGIC_F = "/CRRCSim_airplane/power/automagic/@F";
        public const string XPATH_POWER_AUTOMAGIC_V = "/CRRCSim_airplane/power/automagic/@V";

        public const string XPATH_LAUNCH_ALTITUDE = "/CRRCSim_airplane/launch/@altitude";
        public const string XPATH_LAUNCH_VELOCITY_REL = "/CRRCSim_airplane/launch/@velocity_rel";
        public const string XPATH_LAUNCH_ANGLE = "/CRRCSim_airplane/launch/@angle";

        static public XmlModel DefaultModel { get; private set; }
        static NumberFormatInfo _nfi;
        static XmlModel()
        {
            _nfi = new NumberFormatInfo();
            _nfi.NumberDecimalSeparator = ".";
            _nfi.NumberGroupSeparator = ",";

            DefaultModel = new XmlModel();
            DefaultModel.Load("default.xml");
        }

        public XDocument Xml { get; private set; }

        // **B** The properties 
        public double RefCord { get => getAttr<double>(XPATH_REF_CHORD); set { setAttr<double>(XPATH_REF_CHORD, value); raisePropertyChanged(nameof(RefCord)); } }
        public double RefSpan { get => getAttr<double>(XPATH_REF_SPAN); set { setAttr<double>(XPATH_REF_SPAN, value); raisePropertyChanged(nameof(RefSpan)); } }
        public double RefArea { get => getAttr<double>(XPATH_REF_AREA); set { setAttr<double>(XPATH_REF_AREA, value); raisePropertyChanged(nameof(RefArea)); } }
        public double RefSpeed { get => getAttr<double>(XPATH_REF_SPEED); set { setAttr<double>(XPATH_REF_SPEED, value); raisePropertyChanged(nameof(RefSpeed)); } }
        

        public double MiscAlpha_0 { get => getAttr<double>(XPATH_MISC_ALPHA_0); set { setAttr<double>(XPATH_MISC_ALPHA_0, value); raisePropertyChanged(nameof(MiscAlpha_0)); } }
        public double MiscSpan_eff { get => getAttr<double>(XPATH_MISC_SPAN_EFF); set { setAttr<double>(XPATH_MISC_SPAN_EFF, value); raisePropertyChanged(nameof(MiscSpan_eff)); } }


        public double MCm_0 { get => getAttr<double>(XPATH_M_CM_0); set { setAttr<double>(XPATH_M_CM_0, value); raisePropertyChanged(nameof(MCm_0)); } }
        public double MCm_a { get => getAttr<double>(XPATH_M_CM_A); set { setAttr<double>(XPATH_M_CM_A, value); raisePropertyChanged(nameof(MCm_a)); } }
        public double MCm_q { get => getAttr<double>(XPATH_M_CM_Q); set { setAttr<double>(XPATH_M_CM_Q, value); raisePropertyChanged(nameof(MCm_q)); } }
        public double MCm_de { get => getAttr<double>(XPATH_M_CM_DE); set { setAttr<double>(XPATH_M_CM_DE, value); raisePropertyChanged(nameof(MCm_de)); } }


        public double LiftCL_0 { get => getAttr<double>(XPATH_LIFT_CL_0); set { setAttr<double>(XPATH_LIFT_CL_0, value); raisePropertyChanged(nameof(LiftCL_0)); } }
        public double LiftCL_max { get => getAttr<double>(XPATH_LIFT_CL_MAX); set { setAttr<double>(XPATH_LIFT_CL_MAX, value); raisePropertyChanged(nameof(LiftCL_max)); } }
        public double LiftCL_min { get => getAttr<double>(XPATH_LIFT_CL_MIN); set { setAttr<double>(XPATH_LIFT_CL_MIN, value); raisePropertyChanged(nameof(LiftCL_min)); } }
        public double LiftCL_a { get => getAttr<double>(XPATH_LIFT_CL_A); set { setAttr<double>(XPATH_LIFT_CL_A, value); raisePropertyChanged(nameof(LiftCL_a)); } }
        public double LiftCL_q { get => getAttr<double>(XPATH_LIFT_CL_Q); set { setAttr<double>(XPATH_LIFT_CL_Q, value); raisePropertyChanged(nameof(LiftCL_q)); } }
        public double LiftCL_de { get => getAttr<double>(XPATH_LIFT_CL_DE); set { setAttr<double>(XPATH_LIFT_CL_DE, value); raisePropertyChanged(nameof(LiftCL_de)); } }
        public double LiftCL_drop { get => getAttr<double>(XPATH_LIFT_CL_DROP); set { setAttr<double>(XPATH_LIFT_CL_DROP, value); raisePropertyChanged(nameof(LiftCL_drop)); } }
        public double LiftCL_CD0 { get => getAttr<double>(XPATH_LIFT_CL_CD0); set { setAttr<double>(XPATH_LIFT_CL_CD0, value); raisePropertyChanged(nameof(LiftCL_CD0)); } }



        public double DragCD_prof { get => getAttr<double>(XPATH_DRAG_CD_PROF); set { setAttr<double>(XPATH_DRAG_CD_PROF, value); raisePropertyChanged(nameof(DragCD_prof)); } }
        public double DragUexp_CD { get => getAttr<double>(XPATH_DRAG_UEXP_CD); set { setAttr<double>(XPATH_DRAG_UEXP_CD, value); raisePropertyChanged(nameof(DragUexp_CD)); } }
        public double DragCD_stall { get => getAttr<double>(XPATH_DRAG_CD_STALL); set { setAttr<double>(XPATH_DRAG_CD_STALL, value); raisePropertyChanged(nameof(DragCD_stall)); } }
        public double DragCD_CLsq { get => getAttr<double>(XPATH_DRAG_CD_CLSQ); set { setAttr<double>(XPATH_DRAG_CD_CLSQ, value); raisePropertyChanged(nameof(DragCD_CLsq)); } }
        public double DragCD_AIsq { get => getAttr<double>(XPATH_DRAG_CD_AISQ); set { setAttr<double>(XPATH_DRAG_CD_AISQ, value); raisePropertyChanged(nameof(DragCD_AIsq)); } }
        public double DragCD_ELsq { get => getAttr<double>(XPATH_DRAG_CD_ELSQ); set { setAttr<double>(XPATH_DRAG_CD_ELSQ, value); raisePropertyChanged(nameof(DragCD_ELsq)); } }


        public double YCY_b { get => getAttr<double>(XPATH_Y_CY_B); set { setAttr<double>(XPATH_Y_CY_B, value); raisePropertyChanged(nameof(YCY_b)); } }
        public double YCY_p { get => getAttr<double>(XPATH_Y_CY_P); set { setAttr<double>(XPATH_Y_CY_P, value); raisePropertyChanged(nameof(YCY_p)); } }
        public double YCY_r { get => getAttr<double>(XPATH_Y_CY_R); set { setAttr<double>(XPATH_Y_CY_R, value); raisePropertyChanged(nameof(YCY_r)); } }
        public double YCY_dr { get => getAttr<double>(XPATH_Y_CY_DR); set { setAttr<double>(XPATH_Y_CY_DR, value); raisePropertyChanged(nameof(YCY_dr)); } }
        public double YCY_da { get => getAttr<double>(XPATH_Y_CY_DA); set { setAttr<double>(XPATH_Y_CY_DA, value); raisePropertyChanged(nameof(YCY_da)); } }



        public double lCl_b { get => getAttr<double>(XPATH_L_CL_B); set { setAttr<double>(XPATH_L_CL_B, value); raisePropertyChanged(nameof(lCl_b)); } }
        public double lCl_p { get => getAttr<double>(XPATH_L_CL_P); set { setAttr<double>(XPATH_L_CL_P, value); raisePropertyChanged(nameof(lCl_p)); } }
        public double lCl_r { get => getAttr<double>(XPATH_L_CL_R); set { setAttr<double>(XPATH_L_CL_R, value); raisePropertyChanged(nameof(lCl_r)); } }
        public double lCl_dr { get => getAttr<double>(XPATH_L_CL_DR); set { setAttr<double>(XPATH_L_CL_DR, value); raisePropertyChanged(nameof(lCl_dr)); } }
        public double lCl_da { get => getAttr<double>(XPATH_L_CL_DA); set { setAttr<double>(XPATH_L_CL_DA, value); raisePropertyChanged(nameof(lCl_da)); } }



        public double nCn_b { get => getAttr<double>(XPATH_N_CN_B); set { setAttr<double>(XPATH_N_CN_B, value); raisePropertyChanged(nameof(nCn_b)); } }
        public double nCn_p { get => getAttr<double>(XPATH_N_CN_P); set { setAttr<double>(XPATH_N_CN_P, value); raisePropertyChanged(nameof(nCn_p)); } }
        public double nCn_r { get => getAttr<double>(XPATH_N_CN_R); set { setAttr<double>(XPATH_N_CN_R, value); raisePropertyChanged(nameof(nCn_r)); } }
        public double nCn_dr { get => getAttr<double>(XPATH_N_CN_DR); set { setAttr<double>(XPATH_N_CN_DR, value); raisePropertyChanged(nameof(nCn_dr)); } }
        public double nCn_da { get => getAttr<double>(XPATH_N_CN_DA); set { setAttr<double>(XPATH_N_CN_DA, value); raisePropertyChanged(nameof(nCn_da)); } }



        public double flapsdrag { get => getAttr<double>(XPATH_FLAPS_DRAG); set { setAttr<double>(XPATH_FLAPS_DRAG, value); raisePropertyChanged(nameof(flapsdrag)); } }
        public double flapslift { get => getAttr<double>(XPATH_FLAPS_LIFT); set { setAttr<double>(XPATH_FLAPS_LIFT, value); raisePropertyChanged(nameof(flapslift)); } }
        public double flapsmoment { get => getAttr<double>(XPATH_FLAPS_MOMENT); set { setAttr<double>(XPATH_FLAPS_MOMENT, value); raisePropertyChanged(nameof(flapsmoment)); } }
        public double flapseff_ratio { get => getAttr<double>(XPATH_FLAPS_EFF_RATIO); set { setAttr<double>(XPATH_FLAPS_EFF_RATIO, value); raisePropertyChanged(nameof(flapseff_ratio)); } }


        public double mass_inertiamass { get => getAttr<double>(XPATH_MASS_INERTIA_MASS); set { setAttr<double>(XPATH_MASS_INERTIA_MASS, value); raisePropertyChanged(nameof(mass_inertiamass)); } }
        public double mass_inertiaI_xx { get => getAttr<double>(XPATH_MASS_INERTIA_I_XX); set { setAttr<double>(XPATH_MASS_INERTIA_I_XX, value); raisePropertyChanged(nameof(mass_inertiaI_xx)); } }
        public double mass_inertiaI_yy { get => getAttr<double>(XPATH_MASS_INERTIA_I_YY); set { setAttr<double>(XPATH_MASS_INERTIA_I_YY, value); raisePropertyChanged(nameof(mass_inertiaI_yy)); } }
        public double mass_inertiaI_zz { get => getAttr<double>(XPATH_MASS_INERTIA_I_ZZ); set { setAttr<double>(XPATH_MASS_INERTIA_I_ZZ, value); raisePropertyChanged(nameof(mass_inertiaI_zz)); } }
        public double mass_inertiaI_xz { get => getAttr<double>(XPATH_MASS_INERTIA_I_XZ); set { setAttr<double>(XPATH_MASS_INERTIA_I_XZ, value); raisePropertyChanged(nameof(mass_inertiaI_xz)); } }



        public double power_automagic_F { get => getAttr<double>(XPATH_POWER_AUTOMAGIC_F); set { setAttr<double>(XPATH_POWER_AUTOMAGIC_F, value); raisePropertyChanged(nameof(power_automagic_F)); } }
        public double power_automagic_V { get => getAttr<double>(XPATH_POWER_AUTOMAGIC_V); set { setAttr<double>(XPATH_POWER_AUTOMAGIC_V, value); raisePropertyChanged(nameof(power_automagic_V)); } }



        public double launch_altitude { get => getAttr<double>(XPATH_LAUNCH_ALTITUDE); set { setAttr<double>(XPATH_LAUNCH_ALTITUDE, value); raisePropertyChanged(nameof(launch_altitude)); } }
        public double launch_velocity_rel { get => getAttr<double>(XPATH_LAUNCH_VELOCITY_REL); set { setAttr<double>(XPATH_LAUNCH_VELOCITY_REL, value); raisePropertyChanged(nameof(launch_velocity_rel)); } }
        public double launch_angle { get => getAttr<double>(XPATH_LAUNCH_ANGLE); set { setAttr<double>(XPATH_LAUNCH_ANGLE, value); raisePropertyChanged(nameof(launch_angle)); } }






        // **C** the default values 
        public double DefaultRefCord => DefaultModel.RefCord;
        public double DefaultRefSpan => DefaultModel.RefSpan;
        public double DefaultRefArea => DefaultModel.RefArea;
        public double DefaultRefSpeed => DefaultModel.RefSpeed;



        public double DefaultMiscAlpha_0 => DefaultModel.MiscAlpha_0;
        public double DefaultMiscSpan_eff => DefaultModel.MiscSpan_eff;



        public double DefaultMCm_0 => DefaultModel.MCm_0;
        public double DefaultMCm_a => DefaultModel.MCm_a;
        public double DefaultMCm_q => DefaultModel.MCm_q;
        public double DefaultMCm_de => DefaultModel.MCm_de;



        public double DefaultLiftCL_0 => DefaultModel.LiftCL_0;
        public double DefaultLiftCL_max => DefaultModel.LiftCL_max;
        public double DefaultLiftCL_min => DefaultModel.LiftCL_min;
        public double DefaultLiftCL_a => DefaultModel.LiftCL_a;
        public double DefaultLiftCL_q => DefaultModel.LiftCL_q;
        public double DefaultLiftCL_de => DefaultModel.LiftCL_de;
        public double DefaultLiftCL_drop => DefaultModel.LiftCL_drop;
        public double DefaultLiftCL_CD0 => DefaultModel.LiftCL_CD0;



        public double DefaultDragCD_prof => DefaultModel.DragCD_prof;
        public double DefaultDragUexp_CD => DefaultModel.DragUexp_CD;
        public double DefaultDragCD_stall => DefaultModel.DragCD_stall;
        public double DefaultDragCD_CLsq => DefaultModel.DragCD_CLsq;
        public double DefaultDragCD_AIsq => DefaultModel.DragCD_AIsq;
        public double DefaultDragCD_ELsq => DefaultModel.DragCD_ELsq;



        public double DefaultYCY_b => DefaultModel.YCY_b;
        public double DefaultYCY_p => DefaultModel.YCY_p;
        public double DefaultYCY_r => DefaultModel.YCY_r;
        public double DefaultYCY_dr => DefaultModel.YCY_dr;
        public double DefaultYCY_da => DefaultModel.YCY_da;



        public double DefaultlCl_b => DefaultModel.lCl_b;
        public double DefaultlCl_p => DefaultModel.lCl_p;
        public double DefaultlCl_r => DefaultModel.lCl_r;
        public double DefaultlCl_dr => DefaultModel.lCl_dr;
        public double DefaultlCl_da => DefaultModel.lCl_da;



        public double DefaultnCn_b => DefaultModel.nCn_b;
        public double DefaultnCn_p => DefaultModel.nCn_p;
        public double DefaultnCn_r => DefaultModel.nCn_r;
        public double DefaultnCn_dr => DefaultModel.nCn_dr;
        public double DefaultnCn_da => DefaultModel.nCn_da;




        public double Defaultflapsdrag => DefaultModel.flapsdrag;
        public double Defaultflapslift => DefaultModel.flapslift;
        public double Defaultflapsmoment => DefaultModel.flapsmoment;
        public double Defaultflapseff_ratio => DefaultModel.flapseff_ratio;



        public double Defaultmass_inertiamass => DefaultModel.mass_inertiamass;
        public double Defaultmass_inertiaI_xx => DefaultModel.mass_inertiaI_xx;
        public double Defaultmass_inertiaI_yy => DefaultModel.mass_inertiaI_yy;
        public double Defaultmass_inertiaI_zz => DefaultModel.mass_inertiaI_zz;
        public double Defaultmass_inertiaI_xz => DefaultModel.mass_inertiaI_xz;




        public double Defaultpower_automagic_F => DefaultModel.power_automagic_F;
        public double Defaultpower_automagic_V => DefaultModel.power_automagic_V;




        public double Defaultlaunch_altitude => DefaultModel.launch_altitude;
        public double Defaultlaunch_velocity_rel => DefaultModel.launch_velocity_rel;
        public double Defaultlaunch_angle => DefaultModel.launch_angle;








        public string FilePath { get => getter<string>(); private set => setter(value); }

        public XmlModel()
        {}

        public void Load(string path)
        {
            FilePath = path;
            Xml = XDocument.Load(path);
            raisePropertyChanged();
        }

        private T getAttr<T>(string xPath)
        {
            if (Xml == null)
                return default(T);

            foreach (XAttribute attr in ((IEnumerable)
                Xml.XPathEvaluate(xPath)).OfType<XAttribute>())
            {
                if (typeof(T) == typeof(Double))
                    return (T)Convert.ChangeType(attr.Value, typeof(T), _nfi);
                return (T)Convert.ChangeType(attr.Value, typeof(T));
            }
            return default(T);
        }
        private void setAttr<T>(string xPath, T value)
        {
            if (Xml == null)
                return;

            foreach (XAttribute attr in ((IEnumerable)
                Xml.XPathEvaluate(xPath)).OfType<XAttribute>())
            {
                if (typeof(T) == typeof(Double))
                    attr.Value = Convert.ToDouble(value).ToString(_nfi);
                else
                    attr.Value = value.ToString();
            }
        }

        internal async Task Save()
        {
            Xml.Save(FilePath);
        }

    }
}
