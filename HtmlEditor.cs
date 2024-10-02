using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;

[assembly: WebResource("ParsElecom.HtmlEditor.Images.backcolor.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.bold.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.bulletedlist.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.center.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.forecolor.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.image.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.indent.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.italic.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.justify.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.left.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.link.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.orderedlist.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.outdent.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.pixel.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.right.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.subscript.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.superscript.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.rule.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.underline.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.design.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.html.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.view.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.save.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.new.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.separator.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.Default.Begin.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.Default.Body.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.Default.End.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.Default.separator.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.VisualStudio.Begin.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.VisualStudio.Body.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.VisualStudio.End.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.VisualStudio.separator.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.PE.Begin.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.PE.Body.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.PE.End.bmp", "img/bmp")]
[assembly: WebResource("ParsElecom.HtmlEditor.Images.Toolstrips.PE.separator.gif", "img/gif")]
[assembly: WebResource("ParsElecom.HtmlEditor.Scripts.HtmlEditor.js", "text/javascript")]
[assembly: WebResource("ParsElecom.HtmlEditor.Scripts.DialogBehavior.js", "text/javascript")]
[assembly: WebResource("ParsElecom.HtmlEditor.Dialogs.Blank.htm", "text/html")]
[assembly: WebResource("ParsElecom.HtmlEditor.Dialogs.Link.htm", "text/html")]
[assembly: WebResource("ParsElecom.HtmlEditor.Dialogs.Image.htm", "text/html")]
[assembly: WebResource("ParsElecom.HtmlEditor.Dialogs.Color.htm", "text/html")]
[assembly: CLSCompliant(true)]

namespace ParsElecom.HtmlEditor
{
    /// <summary>
    /// Summary description for HtmlEditor
    /// </summary>
    [ValidationProperty("Text")]
    public class HtmlEditor : CompositeControl, IScriptControl
    {
        protected string htmlText = String.Empty;
        protected HiddenField htmlencodedText;
        protected HiddenField modifiedFlag;
        protected HtmlGenericControl htmlEditor;
        protected HtmlGenericControl designEditor;
        protected HtmlGenericControl htmlArea;
        protected HtmlGenericControl designArea;
        protected HtmlGenericControl toolbarContainer;
        protected HtmlGenericControl htmlTab;
        protected HtmlGenericControl designTab;
        protected Button saveButton;
        protected UpdatePanel updatePanel;
        protected string dialogData;
        protected string toolbarData;
        protected string dialogBoxIDs;
        protected string toolbarIDs;
        protected List<CustomValidator> validators;
        protected bool updateHtml = false;
        protected ScriptManager sm;

        public enum Mode
        {
            Design,
            Html
        }

        public enum Direction
        {
            NotSet,
            LeftToRight,
            RightToLeft
        }

        public enum ToggleModeType
        {
            Tabs,
            ToggleButton,
            Buttons,
            None
        }

        public enum ColorSchemeType
        {
            Custom,
            VisualStudio,
            Default,
            PE
        }

        public enum ToolstripBackgroundImageType
        {
            Default,
            VisualStudio,
            Custom,
            PE
        }

        [Description("The HTML text to edit.")]
        [Category("Data")]
        public string Text
        {
            get
            {
                if (htmlencodedText != null)
                    this.htmlText = ClientDecode(htmlencodedText.Value);

                return this.htmlText;
            }
            set 
            {
                this.htmlText = value;

                if (htmlencodedText == null) return;

                string html = ClientEncode(this.htmlText);
                htmlencodedText.Value = html;

                if (Page.IsPostBack)
                {
                    updatePanel.Update();
                    updateHtml = true;
                }
            }
        }

        [Description("Flag if there are unsaved changes to the text.")]
        [Category("Data")]
        public bool Modified
        {
            get
            {
                if (this.modifiedFlag != null)
                    return this.modifiedFlag.Value == "true";

                return false;
            }
        }

        [Description("Width of the control (in pixels).")]
        [Category("Layout")]
        [DefaultValue("500px")]
        public override Unit Width
        {
            get 
            {
                if (base.Width.IsEmpty)
                    base.Width = Unit.Pixel(500);

                return base.Width; 
            }
            set 
            {
                //if (value.Type != UnitType.Pixel)
                    //throw new HttpException("Width must be set in pixels");

                base.Width = value; 
            }
        }

        [Description("Height of the control (in pixels).")]
        [Category("Layout")]
        [DefaultValue("300px")]
        public override Unit Height
        {
            get 
            {
                if (base.Height.IsEmpty)
                    base.Height = Unit.Pixel(300);

                return base.Height; 
            }
            set
            {
                if (value.Type != UnitType.Pixel)
                    throw new HttpException("Height must be set in pixels");

                if (value.Value < 300) 
                    value = Unit.Pixel(300);
                
                base.Height = value;
            }
        }

        [Description("Image Manager URL.")]
        [Category("Image Manager")]
        [DefaultValue("")]
        public string PURL
        {
            get
            {
                if (ViewState["PURL"] != null)
                    return (string)ViewState["PURL"];
                else
                    return "";
            }
            set
            {
                ViewState["PURL"] = value;
            }
        }

        [Description("Image Manager Token.")]
        [Category("Image Manager")]
        [DefaultValue("")]
        public string Token
        {
            get
            {
                if (ViewState["Token"] != null)
                    return (string)ViewState["Token"];
                else
                    return "";
            }
            set
            {
                ViewState["Token"] = value;
            }
        }

        protected const int defaultEditorBorderSize = 3;
        
        [Description("Size of the border around the editor area.")]
        [Category("Layout")]
        [DefaultValue(defaultEditorBorderSize)]
        public int EditorBorderSize
        {
            get
            {
                if (ViewState["EditorBorderSize"] != null)
                    return (int)ViewState["EditorBorderSize"];
                else
                    return defaultEditorBorderSize;
            }
            set { ViewState["EditorBorderSize"] = value; }
        }

        [Description("Set the color scheme.")]
        [Category("Appearance")]
        [DefaultValue(HtmlEditor.ColorSchemeType.Custom)]
        public HtmlEditor.ColorSchemeType ColorScheme
        {
            get
            {
                if (ViewState["ColorScheme"] != null)
                    return (HtmlEditor.ColorSchemeType)ViewState["ColorScheme"];
                else
                    return HtmlEditor.ColorSchemeType.Custom;
            }
            set { ViewState["ColorScheme"] = value; }
        }

        [Description("Background color of the control.")]
        [Category("Appearance")]
        [DefaultValue("White")]
        public override Color BackColor
        {
            get 
            {
                if (base.BackColor.IsEmpty)
                    base.BackColor = ColorTranslator.FromHtml("White");
                
                return base.BackColor; 
            }
            set { base.BackColor = value; }
        }

        [Description("Default color of the text in the document.")]
        [Category("Appearance")]
        [DefaultValue("Black")]
        public Color EditorForeColor
        {
            get
            {
                if (ViewState["EditorForeColor"] != null)
                    return (Color)ViewState["EditorForeColor"];
                else
                    return ColorTranslator.FromHtml("Black");
            }
            set { ViewState["EditorForeColor"] = value; }
        }

        [Description("Background color of the document.")]
        [Category("Appearance")]
        [DefaultValue("White")]
        public Color EditorBackColor
        {
            get
            {
                if (ViewState["EditorBackColor"] != null)
                    return (Color)ViewState["EditorBackColor"];
                else
                    return ColorTranslator.FromHtml("White");
            }
            set { ViewState["EditorBackColor"] = value; }
        }

        [Description("Color of the border around the editor area.")]
        [Category("Appearance")]
        [DefaultValue("#7f9db9")]
        public Color EditorBorderColor
        {
            get
            {
                if (ViewState["EditorBorderColor"] != null)
                    return (Color)ViewState["EditorBorderColor"];
                else
                    return ColorTranslator.FromHtml("#7f9db9");
            }
            set { ViewState["EditorBorderColor"] = value; }
        }

        [Description("Color of the inner 1px border around the editor area (EditorBorderSize must be greater than 1px).")]
        [Category("Appearance")]
        [DefaultValue("Gray")]
        public Color EditorInnerBorderColor
        {
            get
            {
                if (ViewState["EditorInnerBorderColor"] != null)
                    return (Color)ViewState["EditorInnerBorderColor"];
                else
                    return ColorTranslator.FromHtml("Gray");
            }
            set { ViewState["EditorInnerBorderColor"] = value; }
        }

        [Description("Color of the toolbars and document borders.")]
        [Category("Appearance")]
        [DefaultValue("#7f9db9")]
        public Color ToolbarColor
        {
            get
            {
                if (ViewState["ToolbarColor"] != null)
                    return (Color)ViewState["ToolbarColor"];
                else
                    return ColorTranslator.FromHtml("#7f9db9");
            }
            set { ViewState["ToolbarColor"] = value; }
        }

        [Description("Toolstrip background image type.")]
        [Category("Appearance")]
        [DefaultValue(HtmlEditor.ToolstripBackgroundImageType.Default)]
        public HtmlEditor.ToolstripBackgroundImageType ToolstripBackgroundImage
        {
            get
            {
                if (ViewState["ToolstripBackgroundImage"] != null)
                    return (HtmlEditor.ToolstripBackgroundImageType)ViewState["ToolstripBackgroundImage"];
                else
                    return HtmlEditor.ToolstripBackgroundImageType.Default;
            }
            set { ViewState["ToolstripBackgroundImage"] = value; }
        }

        protected const string defaultToolstripBackgroundImageCustomPath = "";

        [Description("Toolstrip background images custom path.")]
        [Category("Appearance")]
        [DefaultValue(defaultToolstripBackgroundImageCustomPath)]
        public string ToolstripBackgroundImageCustomPath
        {
            get
            {
                if (ViewState["ToolstripBackgroundImageCustomPath"] != null)
                    return (string)ViewState["ToolstripBackgroundImageCustomPath"];
                else
                    return defaultToolstripBackgroundImageCustomPath;
            }
            set { ViewState["ToolstripBackgroundImageCustomPath"] = value; }
        }

        [Description("Prevents display of a toolstrip background image.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool NoToolstripBackgroundImage
        {
            get
            {
                if (ViewState["NoToolstripBackgroundImage"] != null)
                    return (bool)ViewState["NoToolstripBackgroundImage"];
                else
                    return false;
            }
            set { ViewState["NoToolstripBackgroundImage"] = value; }
        }

        [Description("Color of the tab text.")]
        [Category("Appearance")]
        [DefaultValue("Black")]
        public Color TabForeColor
        {
            get
            {
                if (ViewState["TabForeColor"] != null)
                    return (Color)ViewState["TabForeColor"];
                else
                    return ColorTranslator.FromHtml("Black");
            }
            set { ViewState["TabForeColor"] = value; }
        }

        [Description("Background color of the unselected tab.")]
        [Category("Appearance")]
        [DefaultValue("LightSteelBlue")]
        public Color TabBackColor
        {
            get
            {
                if (ViewState["TabBackColor"] != null)
                    return (Color)ViewState["TabBackColor"];
                else
                    return ColorTranslator.FromHtml("LightSteelBlue");
            }
            set { ViewState["TabBackColor"] = value; }
        }

        [Description("Background color of the selected tab.")]
        [Category("Appearance")]
        [DefaultValue("#7f9db9")]
        public Color SelectedTabBackColor
        {
            get
            {
                if (ViewState["SelectedTabBackColor"] != null)
                    return (Color)ViewState["SelectedTabBackColor"];
                else
                    return ColorTranslator.FromHtml("#7f9db9");
            }
            set { ViewState["SelectedTabBackColor"] = value; }
        }

        [Description("Color of the selected tab text.")]
        [Category("Appearance")]
        [DefaultValue("White")]
        public Color SelectedTabTextColor
        {
            get
            {
                if (ViewState["SelectedTabTextColor"] != null)
                    return (Color)ViewState["SelectedTabTextColor"];
                else
                    return ColorTranslator.FromHtml("White");
            }
            set { ViewState["SelectedTabTextColor"] = value; }
        }

        [Description("Background color of the tab on mouseover.")]
        [Category("Appearance")]
        [DefaultValue("LightBlue")]
        public Color TabMouseOverColor
        {
            get
            {
                if (ViewState["TabMouseOverColor"] != null)
                    return (Color)ViewState["TabMouseOverColor"];
                else
                    return ColorTranslator.FromHtml("LightBlue");
            }
            set { ViewState["TabMouseOverColor"] = value; }
        }

        [Description("Background color of the tabbar.")]
        [Category("Appearance")]
        [DefaultValue("White")]
        public Color TabbarBackColor
        {
            get
            {
                if (ViewState["TabbarBackColor"] != null)
                    return (Color)ViewState["TabbarBackColor"];
                else
                    return ColorTranslator.FromHtml("White");
            }
            set { ViewState["TabbarBackColor"] = value; }
        }

        [Description("Background color of the toolbar buttons on mouseover.")]
        [Category("Appearance")]
        [DefaultValue("#c1d2ee")]
        public Color ButtonMouseOverColor
        {
            get
            {
                if (ViewState["ButtonMouseOverColor"] != null)
                    return (Color)ViewState["ButtonMouseOverColor"];
                else
                    return ColorTranslator.FromHtml("#c1d2ee");
            }
            set { ViewState["ButtonMouseOverColor"] = value; }
        }

        [Description("Border color of the toolbar buttons on mouseover.")]
        [Category("Appearance")]
        [DefaultValue("#316ac5")]
        public Color ButtonMouseOverBorderColor
        {
            get
            {
                if (ViewState["ButtonMouseOverBorderColor"] != null)
                    return (Color)ViewState["ButtonMouseOverBorderColor"];
                else
                    return ColorTranslator.FromHtml("#316ac5");
            }
            set { ViewState["ButtonMouseOverBorderColor"] = value; }
        }

        [Description("Background color of the dialog.")]
        [Category("Appearance")]
        [DefaultValue("GhostWhite")]
        public Color DialogBackColor
        {
            get
            {
                if (ViewState["DialogBackColor"] != null)
                    return (Color)ViewState["DialogBackColor"];
                else
                    return ColorTranslator.FromHtml("GhostWhite");
            }
            set { ViewState["DialogBackColor"] = value; }
        }

        [Description("Color of the dialog text.")]
        [Category("Appearance")]
        [DefaultValue("Black")]
        public Color DialogForeColor
        {
            get
            {
                if (ViewState["DialogForeColor"] != null)
                    return (Color)ViewState["DialogForeColor"];
                else
                    return ColorTranslator.FromHtml("Black");
            }
            set { ViewState["DialogForeColor"] = value; }
        }

        [Description("Color of the dialog heading.")]
        [Category("Appearance")]
        [DefaultValue("LightSteelBlue")]
        public Color DialogHeadingColor
        {
            get
            {
                if (ViewState["DialogHeadingColor"] != null)
                    return (Color)ViewState["DialogHeadingColor"];
                else
                    return this.TabBackColor;
            }
            set { ViewState["DialogHeadingColor"] = value; }
        }

        [Description("Color of the dialog heading text.")]
        [Category("Appearance")]
        [DefaultValue("Black")]
        public Color DialogHeadingTextColor
        {
            get
            {
                if (ViewState["DialogHeadingTextColor"] != null)
                    return (Color)ViewState["DialogHeadingTextColor"];
                else
                    return this.TabForeColor;
            }
            set { ViewState["DialogHeadingTextColor"] = value; }
        }

        [Description("Color of the dialog button bar.")]
        [Category("Appearance")]
        [DefaultValue("LightSteelBlue")]
        public Color DialogButtonBarColor
        {
            get
            {
                if (ViewState["DialogButtonBarColor"] != null)
                    return (Color)ViewState["DialogButtonBarColor"];
                else
                    return this.TabBackColor;
            }
            set { ViewState["DialogButtonBarColor"] = value; }
        }

        [Description("Color of the dialog table.")]
        [Category("Appearance")]
        [DefaultValue("#eeeeee")]
        public Color DialogTableColor
        {
            get
            {
                if (ViewState["DialogTableColor"] != null)
                    return (Color)ViewState["DialogTableColor"];
                else
                    return ColorTranslator.FromHtml("#eeeeee");
            }
            set { ViewState["DialogTableColor"] = value; }
        }

        [Description("Color of the dialog tab text.")]
        [Category("Appearance")]
        [DefaultValue("Black")]
        public Color DialogTabTextColor
        {
            get
            {
                if (ViewState["DialogTabTextColor"] != null)
                    return (Color)ViewState["DialogTabTextColor"];
                else
                    return this.TabForeColor;
            }
            set { ViewState["DialogTabTextColor"] = value; }
        }

        [Description("Color of the dialog selected tab text.")]
        [Category("Appearance")]
        [DefaultValue("White")]
        public Color DialogSelectedTabTextColor
        {
            get
            {
                if (ViewState["DialogSelectedTabTextColor"] != null)
                    return (Color)ViewState["DialogSelectedTabTextColor"];
                else
                    return this.SelectedTabTextColor;
            }
            set { ViewState["DialogSelectedTabTextColor"] = value; }
        }

        [Description("Color of the dialog unselected tab.")]
        [Category("Appearance")]
        [DefaultValue("LightSteelBlue")]
        public Color DialogUnselectedTabColor
        {
            get
            {
                if (ViewState["DialogUnselectedTabColor"] != null)
                    return (Color)ViewState["DialogUnselectedTabColor"];
                else
                    return this.TabBackColor;
            }
            set { ViewState["DialogUnselectedTabColor"] = value; }
        }

        [Description("Color of the dialog selected tab.")]
        [Category("Appearance")]
        [DefaultValue("#7f9db9")]
        public Color DialogSelectedTabColor
        {
            get
            {
                if (ViewState["DialogSelectedTabColor"] != null)
                    return (Color)ViewState["DialogSelectedTabColor"];
                else
                    return this.SelectedTabBackColor;
            }
            set { ViewState["DialogSelectedTabColor"] = value; }
        }

        [Description("Color of the dialog borders.")]
        [Category("Appearance")]
        [DefaultValue("Black")]
        public Color DialogBorderColor
        {
            get
            {
                if (ViewState["DialogBorderColor"] != null)
                    return (Color)ViewState["DialogBorderColor"];
                else
                    return ColorTranslator.FromHtml("Black");
            }
            set { ViewState["DialogBorderColor"] = value; }
        }

        [Description("Convert output to XHTML.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool OutputXHTML
        {
            get
            {
                if (ViewState["OutputXHTML"] != null)
                    return (bool)ViewState["OutputXHTML"];
                else
                    return true;
            }
            set { ViewState["OutputXHTML"] = value; }
        }

        [Description("Convert deprecated syntax to standards (OutputXHTML must be true).")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ConvertDeprecatedSyntax
        {
            get
            {
                if (ViewState["ConvertDeprecatedSyntax"] != null)
                    return (bool)ViewState["ConvertDeprecatedSyntax"];
                else
                    return true;
            }
            set { ViewState["ConvertDeprecatedSyntax"] = value; }
        }

        [Description("Convert top-level paragraph elements inserted in Internet Explorer to appropriate break and/or div elements (OutputXHTML and ConvertDeprecatedSyntax must be true).")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ConvertParagraphs
        {
            get
            {
                if (ViewState["ConvertParagraphs"] != null)
                    return (bool)ViewState["ConvertParagraphs"];
                else
                    return false;
            }
            set { ViewState["ConvertParagraphs"] = value; }
        }

        [Description("Replace nbsp entities inserted in Internet Explorer with spaces (OutputXHTML must be true).")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReplaceNoBreakSpace
        {
            get
            {
                if (ViewState["ReplaceNoBreakSpace"] != null)
                    return (bool)ViewState["ReplaceNoBreakSpace"];
                else
                    return false;
            }
            set { ViewState["ReplaceNoBreakSpace"] = value; }
        }

        [Description("Display an asterick on the current tab whenever the text has been modified and not yet saved.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowModifiedAsterick
        {
            get
            {
                if (ViewState["ShowModifiedAsterick"] != null)
                    return (bool)ViewState["ShowModifiedAsterick"];
                else
                    return true;
            }
            set { ViewState["ShowModifiedAsterick"] = value; }
        }

        [Description("Add newline character after block tags in the HTML mode editor (OutputXHTML must be true, IE only).")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool FormatHtmlMode
        {
            get
            {
                if (ViewState["FormatHtmlMode"] != null)
                    return (bool)ViewState["FormatHtmlMode"];
                else
                    return true;
            }
            set { ViewState["FormatHtmlMode"] = value; }
        }

        protected const string defaultAllowedTags = "p,b,i,u,em,big,small,div,img,span,blockquote,code,pre,br,hr,ul,ol,li,del,ins,strong,a,font,dl,dd,dt,h1,h2,h3,h4,h5,h6,address,sub,sup";

        [Description("Comma delimited list of allowed tags (OutputXHTML must be true).")]
        [Category("Behavior")]
        [DefaultValue(defaultAllowedTags)]
        public string AllowedTags
        {
            get
            {
                if (ViewState["AllowedTags"] == null)
                {
                    ViewState["AllowedTags"] = defaultAllowedTags;
                }

                return (string)ViewState["AllowedTags"];
            }
            set
            {
                if (value != null)
                {
                    string[] allowedTags = value.Split(',');
                    if (allowedTags.Length > 0)
                        ViewState["AllowedTags"] = BuildCommaDelimitedStringFromList(allowedTags, true);
                }
            }
        }

        protected const string defaultAllowedAttributes = "class,href,target,src,align,valign,color,size,style,face,dir,alt,title";

        [Description("Comma delimited list of allowed attributes (OutputXHTML must be true).")]
        [Category("Behavior")]
        [DefaultValue(defaultAllowedAttributes)]
        public string AllowedAttributes
        {
            get
            {
                if (ViewState["AllowedAttributes"] == null)
                {
                    ViewState["AllowedAttributes"] = defaultAllowedAttributes;
                }

                return (string)ViewState["AllowedAttributes"];
            }
            set
            {
                if (value != null)
                {
                    string[] allowedAttributes = value.Split(',');
                    if (allowedAttributes.Length > 0)
                        ViewState["AllowedAttributes"] = BuildCommaDelimitedStringFromList(allowedAttributes, true);
                }
            }
        }

        protected const string defaultNoScriptAttributes = "href,src";

        [Description("Comma delimited list of no script attributes (OutputXHTML must be true).")]
        [Category("Behavior")]
        [DefaultValue(defaultNoScriptAttributes)]
        public string NoScriptAttributes
        {
            get
            {
                if (ViewState["NoScriptAttributes"] == null)
                {
                    ViewState["NoScriptAttributes"] = defaultNoScriptAttributes;
                }

                return (string)ViewState["NoScriptAttributes"];
            }
            set
            {
                if (value != null)
                {
                    string[] noScriptAttributes = value.Split(',');
                    if (noScriptAttributes.Length > 0)
                        ViewState["NoScriptAttributes"] = BuildCommaDelimitedStringFromList(noScriptAttributes, true);
                }
            }
        }

        [Description("Initial mode on startup.")]
        [Category("Behavior")]
        [DefaultValue(HtmlEditor.Mode.Design)]
        public HtmlEditor.Mode InitialMode
        {
            get
            {
                if (ViewState["InitialMode"] != null)
                    return (HtmlEditor.Mode)ViewState["InitialMode"];
                else
                    return HtmlEditor.Mode.Design;
            }
            set { ViewState["InitialMode"] = value; }
        }

        [Description("Determines if Design Mode is editable.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool DesignModeEditable
        {
            get
            {
                if (ViewState["DesignModeEditable"] != null)
                    return (bool)ViewState["DesignModeEditable"];
                else
                    return true;
            }
            set { ViewState["DesignModeEditable"] = value; }
        }

        [Description("Determines if Html Mode is editable.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool HtmlModeEditable
        {
            get
            {
                if (ViewState["HtmlModeEditable"] != null)
                    return (bool)ViewState["HtmlModeEditable"];
                else
                    return true;
            }
            set { ViewState["HtmlModeEditable"] = value; }
        }

        [Description("Determines the mechanism for switching between modes.")]
        [Category("Behavior")]
        [DefaultValue(HtmlEditor.ToggleModeType.Tabs)]
        public HtmlEditor.ToggleModeType ToggleMode
        {
            get
            {
                if (ViewState["ToggleMode"] != null)
                    return (HtmlEditor.ToggleModeType)ViewState["ToggleMode"];
                else
                    return HtmlEditor.ToggleModeType.Tabs;
            }
            set { ViewState["ToggleMode"] = value; }
        }

        [Description("Determines if the toolbar is docked to the editor.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ToolbarDocked
        {
            get
            {
                if (ViewState["ToolbarDocked"] != null)
                    return (bool)ViewState["ToolbarDocked"];
                else
                    return true;
            }
            set { ViewState["ToolbarDocked"] = value; }
        }

        protected const string defaultToolbarClass = "";
        
        [Description("Assigns a CSS class to the toolbar (ToolbarDocked must be false).")]
        [Category("Behavior")]
        [DefaultValue(defaultToolbarClass)]
        public string ToolbarClass
        {
            get
            {
                if (ViewState["ToolbarClass"] != null)
                    return (string)ViewState["ToolbarClass"];
                else
                    return defaultToolbarClass;
            }
            set { ViewState["ToolbarClass"] = value.Trim(); }
        }

        [Description("Css file to link for Design mode.")]
        [Category("Appearance")]
        [DefaultValue("")]
        public string DesignModeCss
        {
            get
            {
                if (ViewState["DesignModeCss"] != null)
                    return (string)ViewState["DesignModeCss"];
                else
                    return String.Empty;
            }
            set { ViewState["DesignModeCss"] = value; }
        }

        [Description("Determines if the Design Mode document includes the meta tag for emulating IE 7 in IE8 and higher.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool DesignModeEmulateIE7
        {
            get
            {
                if (ViewState["DesignModeEmulateIE7"] != null)
                    return (bool)ViewState["DesignModeEmulateIE7"];
                else
                    return true;
            }
            set { ViewState["DesignModeEmulateIE7"] = value; }
        }

        [Description("Text direction in Design mode.")]
        [Category("Behavior")]
        [DefaultValue(HtmlEditor.Direction.NotSet)]
        public HtmlEditor.Direction TextDirection
        {
            get
            {
                if (ViewState["TextDirection"] != null)
                    return (HtmlEditor.Direction)ViewState["TextDirection"];
                else
                    return HtmlEditor.Direction.NotSet;
            }
            set { ViewState["TextDirection"] = value; }
        }

        [Description("Client-side handler for modified flag change.")]
        [Category("Behavior")]
        public String ModifiedChanged
        {
            get
            {
                if (ViewState["ModifiedChanged"] != null)
                    return (String)ViewState["ModifiedChanged"];
                else
                    return String.Empty;
            }
            set { ViewState["ModifiedChanged"] = value; }
        }

        [Description("Client-side handler for design mode context change.")]
        [Category("Behavior")]
        public String ContextChanged
        {
            get
            {
                if (ViewState["ContextChanged"] != null)
                    return (String)ViewState["ContextChanged"];
                else
                    return String.Empty;
            }
            set { ViewState["ContextChanged"] = value; }
        }

        protected const string defaultToolbars = "Select#Format,Select#Font,Select#Size:ForeColor,BackColor;Bold,Italic,Underline:Left,Center,Right,Justify|OrderedList,BulletedList|Indent,Outdent|Rule|Subscript,Superscript:Link,Image";

        [Description("Definition of toolbar elements.")]
        [Category("Behavior")]
        [DefaultValue(defaultToolbars)]
        public string Toolbars
        {
            get
            {
                if (ViewState["Toolbars"] == null)
                {
                    ViewState["Toolbars"] = defaultToolbars;
                }

                return (string)ViewState["Toolbars"];
            }
            set
            {
                if (value != null)
                {
                    string toolbars = String.Empty;
                    string[] listToolbars = value.Split(';');

                    foreach (string toolbar in listToolbars)
                    {
                        string toolbarElements = BuildCommaDelimitedStringFromList(toolbar.Split(','), false);
                        
                        if (toolbars.Length > 0) toolbars += ";";
                        toolbars += toolbarElements;
                    }

                    ViewState["Toolbars"] = toolbars;
                }
            }
        }

        [Description("Determines if dialogs can be moved.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool DialogFloatingBehavior
        {
            get
            {
                if (ViewState["DialogFloatingBehavior"] != null)
                    return (bool)ViewState["DialogFloatingBehavior"];
                else
                    return true;
            }
            set { ViewState["DialogFloatingBehavior"] = value; }
        }

        [Description("Determines if modifications are automatically saved as part of client-side validation (CausesValidation must be true for the control clicked).")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AutoSave
        {
            get
            {
                if (ViewState["AutoSave"] != null)
                    return (bool)ViewState["AutoSave"];
                else
                    return true;
            }
            set { ViewState["AutoSave"] = value; }
        }

        protected const string defaultAutoSaveValidationGroups = "*";

        [Description("Comma delimited list of ValidationGroup names used by AutoSave (use '*' for all, '.' for default).")]
        [Category("Behavior")]
        [DefaultValue(defaultAutoSaveValidationGroups)]
        public string AutoSaveValidationGroups
        {
            get
            {
                object o = ViewState["AutoSaveValidationGroups"];
                if (o != null) return (string)o;

                ViewState["AutoSaveValidationGroups"] = defaultAutoSaveValidationGroups;
                return defaultAutoSaveValidationGroups;
            }
            set
            {
                if (value != null)
                {
                    string[] autosaveValidationGroups = value.Split(',');
                    if (autosaveValidationGroups.Length > 0)
                        ViewState["AutoSaveValidationGroups"] = BuildCommaDelimitedStringFromList(autosaveValidationGroups, false);
                }
            }
        }

        [Description("Comma delimited list of button ID's requiring Save behavior on a client click (Default search pattern is relative to editor location).")]
        [Category("Behavior")]
        public string SaveButtons
        {
            get
            {
                if (ViewState["SaveButtons"] == null)
                {
                    ViewState["SaveButtons"] = String.Empty;
                }

                return (string)ViewState["SaveButtons"];
            }
            set
            {
                if (value != null)
                {
                    string[] saveButtons = value.Split(',');
                    if (saveButtons.Length > 0)
                        ViewState["SaveButtons"] = BuildCommaDelimitedStringFromList(saveButtons, false);
                }
            }
        }

        public void Revert()
        {
            Text = Text;
        }

        public void AttachSaveButton(Control control)
        {
            SaveButtonInfo.AttachButton(this, control);
        }

        protected bool InternetExplorer
        {
            get { return this.DesignMode ? true : this.Page.Request.Browser.IsBrowser("IE"); }
        }

        protected int BrowserVersion
        {
            get { return this.DesignMode ? 0 : this.Page.Request.Browser.MajorVersion; }
        }

        private int editorWidth = 0;

        protected int EditorWidth
        {
            get { return this.editorWidth; }
            set { this.editorWidth = value; }
        }

        private int editorHeight = 0;

        protected int EditorHeight
        {
            get { return this.editorHeight; }
            set { this.editorHeight = value; }
        }

        private int toolbarHeight = 25;

        protected int ToolbarHeight
        {
            get { return this.toolbarHeight; }
            set { this.toolbarHeight = value; }
        }

        protected int ToolbarPadding
        {
            get { return Math.Max(defaultEditorBorderSize, EditorBorderSize); }
        }

        protected int AdditionalToolbarPadding
        {
            get { return ToolbarPadding - EditorBorderSize; }
        }

        protected int ToolstripPadding
        {
            get { return 3; }
        }

        private int tabbarHeight = 30;

        protected int TabbarHeight
        {
            get { return this.tabbarHeight; }
            set { this.tabbarHeight = value; }
        }

        private int tabHeight = 24;

        protected int TabHeight
        {
            get { return this.tabHeight; }
            set { this.tabHeight = value; }
        }

        private int tabWidth = 80;

        protected int TabWidth
        {
            get { return this.tabWidth; }
            set { this.tabWidth = value; }
        }

        private int dialogHeadingHeight = 30;

        protected int DialogHeadingHeight
        {
            get { return this.dialogHeadingHeight; }
            set { this.dialogHeadingHeight = value; }
        }

        private int dialogButtonBarHeight = 30;

        protected int DialogButtonBarHeight
        {
            get { return this.dialogButtonBarHeight; }
            set { this.dialogButtonBarHeight = value; }
        }

        protected bool HasDialog(string name)
        {
            string[] dialogs = this.dialogData.Split(';');

            foreach (string dialog in dialogs)
            {
                string[] dialogData = dialog.Split(',');
                if (dialogData[0] == name) return true;
            }

            return false;
        }

        protected string PixelImageUrl
        {
            get { return this.GetWebResourceUrl("ParsElecom.HtmlEditor.Images.pixel.gif"); }
        }

        protected String[] ValidationGroups
        {
            get
            {
                object o = ViewState["ValidationGroups"];
                if (o != null) return (String[])o;

                return GetStringList(String.Empty);
            }
            set { ViewState["ValidationGroups"] = value; }
        }

        protected string BuildCommaDelimitedStringFromList(string[] list, bool forceLowerCase)
        {
            string s = String.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                if (i > 0) s += ",";
                
                string item = list[i].Trim();
                if (forceLowerCase) item = item.ToLowerInvariant();

                s += item;
            }
            
            return s;
        }

        protected string ToBackColor
        {
            get { return ColorTranslator.ToHtml(this.BackColor); }
        }

        protected string ToEditorForeColor
        {
            get { return ColorTranslator.ToHtml(this.EditorForeColor); }
        }

        protected string ToEditorBackColor
        {
            get { return ColorTranslator.ToHtml(this.EditorBackColor); }
        }

        protected string ToEditorBorderColor
        {
            get { return ColorTranslator.ToHtml(this.EditorBorderColor); }
        }

        protected string ToEditorInnerBorderColor
        {
            get { return ColorTranslator.ToHtml(this.EditorInnerBorderColor); }
        }

        protected string ToToolbarColor
        {
            get { return ColorTranslator.ToHtml(this.ToolbarColor); }
        }

        protected string ToTabForeColor
        {
            get { return ColorTranslator.ToHtml(this.TabForeColor); }
        }

        protected string ToTabBackColor
        {
            get { return ColorTranslator.ToHtml(this.TabBackColor); }
        }

        protected string ToSelectedTabBackColor
        {
            get { return ColorTranslator.ToHtml(this.SelectedTabBackColor); }
        }

        protected string ToSelectedTabTextColor
        {
            get { return ColorTranslator.ToHtml(this.SelectedTabTextColor); }
        }

        protected string ToTabbarBackColor
        {
            get { return ColorTranslator.ToHtml(this.TabbarBackColor); }
        }

        protected string ToButtonMouseOverColor
        {
            get { return ColorTranslator.ToHtml(this.ButtonMouseOverColor); }
        }

        protected string ToDialogBackColor
        {
            get { return ColorTranslator.ToHtml(this.DialogBackColor); }
        }

        protected string ToDialogForeColor
        {
            get { return ColorTranslator.ToHtml(this.DialogForeColor); }
        }

        protected string ToDialogHeadingColor
        {
            get { return ColorTranslator.ToHtml(this.DialogHeadingColor); }
        }

        protected string ToDialogHeadingTextColor
        {
            get { return ColorTranslator.ToHtml(this.DialogHeadingTextColor); }
        }

        protected string ToDialogButtonBarColor
        {
            get { return ColorTranslator.ToHtml(this.DialogButtonBarColor); }
        }

        protected string ToDialogBorderColor
        {
            get { return ColorTranslator.ToHtml(this.DialogBorderColor); }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        protected string FindScriptObject
        {
            get { return "$find('" + this.ClientID + "')"; }
        }

        protected string GetSafeScriptMethod(string method)
        {
            return "var e = " + FindScriptObject + "; if (e) { e." + method + "; }";
        }

        protected string GetWebResourceUrl(string resource)
        {
            return Page.ClientScript.GetWebResourceUrl(typeof(ParsElecom.HtmlEditor.HtmlEditor), resource) + "&purl=" + (string)ViewState["PURL"] + "&token=" + (string)ViewState["Token"];
        }

        protected string GetEncodedWebResourceUrl(string resource)
        {
            return HttpUtility.HtmlEncode(Page.ClientScript.GetWebResourceUrl(typeof(ParsElecom.HtmlEditor.HtmlEditor), resource));
        }

        protected string ClientEncode(string html)
        {
            html = html.Replace("&quot;", "&quotx;");
            html = html.Replace("\"", "&quot;");
            html = html.Replace("&amp;", "&ampx;");
            html = html.Replace("&", "&amp;");
            html = html.Replace("&lt;", "&ltx;");
            html = html.Replace("<", "&lt;");
            html = html.Replace("&gt;", "&gtx;");
            html = html.Replace(">", "&gt;");

            return html;
        }

        protected string ClientDecode(string html)
        {
            html = html.Replace("&gt;", ">");
            html = html.Replace("&gtx;", "&gt;");
            html = html.Replace("&lt;", "<");
            html = html.Replace("&ltx;", "&lt;");
            html = html.Replace("&amp;", "&");
            html = html.Replace("&ampx;", "&amp;");
            html = html.Replace("&quot;", "\"");
            html = html.Replace("&quotx;", "&quot;");

            return html;
        }

        protected string[] GetStringList(string commaDelimitedList)
        {
            string[] stringList = commaDelimitedList.Split(',');
            if (stringList.Length == 1 && stringList[0] == String.Empty)
                return new List<String>().ToArray();

            return stringList;
        }

        protected string GetClientIDs(Control[] controls)
        {
            string clientIDs = String.Empty;

            foreach (Control control in controls)
            {
                if (clientIDs.Length > 0)
                    clientIDs += ";";

                clientIDs += control.ClientID;
            }

            return clientIDs;
        }

        protected string GetDesignModeCss()
        {
            if (DesignModeCss.Length == 0)
                return String.Empty;
            else
                return this.Page.ResolveClientUrl(DesignModeCss);
        }

        protected void CreateDialogBox()
        {
            HtmlGenericControl dialogShim = new HtmlGenericControl("div");
            dialogShim.Style["position"] = "relative";
            dialogShim.Style["right"] = "0px";
            dialogShim.Style["top"] = "0px";
            dialogShim.Style["height"] = "0px";
            dialogShim.Style["width"] = "0px";
            dialogShim.Style["z-index"] = "1";

            Panel dialogBox = new Panel();
            dialogBox.ID = "DialogBox";
            dialogBox.Style["display"] = this.DesignMode ? "block" : "none";
            dialogBox.Style["position"] = "absolute";
            dialogBox.Style["right"] = "50px";
            dialogBox.Style["top"] = "75px";
            dialogBox.Style["height"] = "200px";
            dialogBox.Style["width"] = "300px";
            dialogBox.Style["border"] = "solid 1px " + this.ToDialogBorderColor;
            dialogBox.Style["background-color"] = this.ToDialogBackColor;
            dialogBox.Style["color"] = this.ToDialogForeColor;
            dialogBox.Style["z-index"] = "2";

            HtmlGenericControl dialogHeading = new HtmlGenericControl("div");
            dialogHeading.ID = "DialogHeading";
            dialogHeading.Style["position"] = "relative";
            dialogHeading.Style["right"] = "0px";
            dialogHeading.Style["top"] = "0px";
            dialogHeading.Style["height"] = this.DialogHeadingHeight.ToString() + "px";
            dialogHeading.Style["line-height"] = "29px";
            dialogHeading.Style["font-family"] = "Tahoma";
            dialogHeading.Style["font-size"] = "11pt";
            dialogHeading.Style["font-weight"] = "bold";
            dialogHeading.Style["text-align"] = "center";
            dialogHeading.Style["background-color"] = this.ToDialogHeadingColor;
            dialogHeading.Style["color"] = this.ToDialogHeadingTextColor;
            dialogHeading.Style["border-bottom"] = "solid 1px " + this.ToDialogBorderColor;

            if (this.DialogFloatingBehavior)
            {
                dialogHeading.Attributes["onmousedown"] = this.GetSafeScriptMethod("Drag(this.parentNode, event)");
                dialogHeading.Style["cursor"] = "move";
            }

            string frameTag = this.DesignMode ? "div" : "iframe";

            HtmlGenericControl dialogFrame = new HtmlGenericControl(frameTag);
            dialogFrame.ID = "DialogFrame";
            dialogFrame.Style["display"] = "block";
            dialogFrame.Style["background-color"] = this.ToDialogBackColor;

            if (this.DesignMode)
            {
                int frameHeight = 200 - this.DialogHeadingHeight - this.DialogButtonBarHeight - 2;
                dialogFrame.Style["height"] = frameHeight.ToString() + "px";
            }
            else
            {
                dialogFrame.Attributes["width"] = "100%";
                dialogFrame.Attributes["frameborder"] = "0";
            }

            HtmlGenericControl dialogButtons = new HtmlGenericControl("div");
            dialogButtons.ID = "DialogButtons";
            dialogButtons.Style["height"] = this.DialogButtonBarHeight.ToString() + "px";
            dialogButtons.Style["text-align"] = "center";
            dialogButtons.Style["border-top"] = "solid 1px " + this.ToDialogBorderColor;
            dialogButtons.Style["background-color"] = this.ToDialogButtonBarColor;

            Button dialogOKButton = new Button();
            dialogOKButton.ID = "DialogOKButton";
            dialogOKButton.Text = "تایید";
            dialogOKButton.OnClientClick = GetSafeScriptMethod("OnDialogOK()") + "; return false;";
            dialogOKButton.Style["width"] = "75px";
            dialogOKButton.Style["font-family"] = "Tahoma";
            dialogOKButton.Style["font-size"] = "8pt";
            dialogOKButton.Style["margin-top"] = "5px";

            Button dialogCancelButton = new Button();
            dialogCancelButton.ID = "DialogCancelButton";
            dialogCancelButton.Text = "لغو";
            dialogCancelButton.OnClientClick = GetSafeScriptMethod("CloseDialogBox()") + "; return false;";
            dialogCancelButton.Style["width"] = "75px";
            dialogCancelButton.Style["font-family"] = "Tahoma";
            dialogCancelButton.Style["font-size"] = "8pt";
            dialogCancelButton.Style["margin-top"] = "5px";

            dialogButtons.Controls.Add(dialogOKButton);
            dialogButtons.Controls.Add(dialogCancelButton);

            dialogBox.Controls.Add(dialogHeading);
            dialogBox.Controls.Add(dialogFrame);
            dialogBox.Controls.Add(dialogButtons);

            dialogShim.Controls.Add(dialogBox);

            this.Controls.Add(dialogShim);

            Control[] controls = { dialogShim, dialogBox, dialogHeading, dialogFrame };
            this.dialogBoxIDs = GetClientIDs(controls);
        }

        protected HtmlImage CreateRoundedCorner(bool left, bool top, string color)
        {
            string leftRight = left ? "left" : "right";
            string topBottom = top ? "top" : "bottom";
            string leftRightPosition = "0px";
            string topBottomPosition = "0px";

            if (!this.DesignMode)
            {
                bool ie6 = this.InternetExplorer &&
                           this.Page.Request.Browser.MajorVersion < 7;

                if ((ie6) && (!top) && (!left)) leftRightPosition = "-1px";
            }
            
            HtmlImage corner = new HtmlImage();
            string resource = "ParsElecom.HtmlEditor.Images.pixel.gif";
            corner.Src = GetEncodedWebResourceUrl(resource);
            corner.Height = 1;
            corner.Width = 1;
            corner.Attributes["alt"] = String.Empty;
            corner.Style["position"] = "absolute";
            corner.Style[leftRight] = leftRightPosition;
            corner.Style[topBottom] = topBottomPosition;
            corner.Style["margin"] = "0px";
            corner.Style["padding"] = "0px";
            corner.Style["background-color"] = color;
            corner.Style["font-size"] = "0px";

            return corner;
        }

        protected SelectInfo.SelectList CreateSelectList(string name)
        {
            SelectInfo.SelectList selectList = new SelectInfo.SelectList();
            
            switch (name)
            {
                case "Format" :
                    selectList.Add("قالب", "p");
                    if (!this.ConvertParagraphs) selectList.Add("پاراگراف", "p");
                    selectList.Add("سرصفحه 1", "h1");
                    selectList.Add("سرصفحه 2", "h2");
                    selectList.Add("سرصفحه 3", "h3");
                    selectList.Add("سرصفحه 4", "h4");
                    selectList.Add("سرصفحه 5", "h5");
                    selectList.Add("سرصفحه 6", "h6");
                    selectList.Add("آدرس", "address");
                    selectList.Add("قالب بندی", "pre");
                    break;
                case "Font" :
                    selectList.Add("فونت", "");
                    selectList.Add("Arial", "Arial");
                    selectList.Add("Courier New", "Courier New");
                    selectList.Add("Garamond", "Garamond");
                    selectList.Add("Sans Serif", "Sans Serif");
                    selectList.Add("Tahoma", "Tahoma");
                    selectList.Add("Times", "Times New Roman");
                    selectList.Add("Verdana", "Verdana");
                    selectList.Add("Wingdings", "Wingdings");
                    break;
                case "Size":
                    selectList.Add("سایز", "");
                    selectList.Add("بسیار کوچک", "1");
                    selectList.Add("کمی کوچک", "2");
                    selectList.Add("کوچک", "3");
                    selectList.Add("متوسط", "4");
                    selectList.Add("بزرگ", "5");
                    selectList.Add("کمی بزرگ", "6");
                    selectList.Add("بسیار بزرگ", "7");
                    break;
                default:
                    throw new HttpException(String.Format("Invalid select list name: {0}", name));
            }

            return selectList;
        }

        protected ButtonInfo CreateButtonInfo(string name)
        {
            switch (name)
            {
                case "ForeColor" :
                    return new ButtonInfo(this, name, "رنگ متن");
                case "BackColor" :
                    return new ButtonInfo(this, name, "رنگ زمینه");
                case "Bold":
                    return new ButtonInfo(this, name, "درشت");
                case "Italic":
                    return new ButtonInfo(this, name, "کچ");
                case "Underline":
                    return new ButtonInfo(this, name, "خط زیر");
                case "Left":
                    return new ButtonInfo(this, name, "چپ چین");
                case "Center":
                    return new ButtonInfo(this, name, "وسط چین");
                case "Right":
                    return new ButtonInfo(this, name, "راست چین");
                case "Justify":
                    return new ButtonInfo(this, name, "کامل");
                case "OrderedList":
                    return new ButtonInfo(this, name, "لیست عددی");
                case "BulletedList":
                    return new ButtonInfo(this, name, "لیست عادی");
                case "Indent":
                    return new ButtonInfo(this, name, "به عقب");
                case "Outdent":
                    return new ButtonInfo(this, name, "به جلو");
                case "Rule":
                    return new ButtonInfo(this, name, "خط افقی");
                case "Subscript":
                    return new ButtonInfo(this, name, "پایین نویس");
                case "Superscript":
                    return new ButtonInfo(this, name, "بالا نویس");
                case "Link":
                    return new ButtonInfo(this, name, "لینک");
                case "Image":
                    return new ButtonInfo(this, name, "عکس");
                case "Design":
                    ButtonInfo designButtonInfo = new ButtonInfo(this, name, "نوشتار");
                    designButtonInfo.Enabled = ToolbarElement.EnabledType.Html;
                    designButtonInfo.Dependency = ToolbarElement.DependencyType.None;
                    designButtonInfo.DisableMethod = ToggleMode == ToggleModeType.ToggleButton ? ToolbarElement.DisableMethodType.Hide : ToolbarElement.DisableMethodType.Opacity;
                    return designButtonInfo;
                case "Html":
                    string htmlToolTip = this.HtmlModeEditable ? "مشاهده کد" : "پیش نمایش";
                    ButtonInfo htmlButtonInfo = new ButtonInfo(this, name, htmlToolTip);
                    htmlButtonInfo.Enabled = ToolbarElement.EnabledType.Design;
                    htmlButtonInfo.Dependency = ToolbarElement.DependencyType.None;
                    htmlButtonInfo.DisableMethod = ToggleMode == ToggleModeType.ToggleButton ? ToolbarElement.DisableMethodType.Hide : ToolbarElement.DisableMethodType.Opacity;
                    return htmlButtonInfo;
                case "View":
                    ButtonInfo viewButtonInfo = new ButtonInfo(this, name, "پیش نمایش");
                    viewButtonInfo.Enabled = ToolbarElement.EnabledType.Html;
                    viewButtonInfo.Dependency = ToolbarElement.DependencyType.None;
                    viewButtonInfo.DisableMethod = ToggleMode == ToggleModeType.ToggleButton ? ToolbarElement.DisableMethodType.Hide : ToolbarElement.DisableMethodType.Opacity;
                    return viewButtonInfo;
                case "Save":
                    ButtonInfo saveButtonInfo = new ButtonInfo(this, name, "ذخیره");
                    saveButtonInfo.Enabled = ToolbarElement.EnabledType.Never;
                    saveButtonInfo.Dependency = ToolbarElement.DependencyType.Editable;
                    return saveButtonInfo;
                case "New":
                    ButtonInfo newButtonInfo = new ButtonInfo(this, name, "جدید");
                    newButtonInfo.Enabled = ToolbarElement.EnabledType.Always;
                    newButtonInfo.Dependency = ToolbarElement.DependencyType.Editable;
                    return newButtonInfo;
                default:
                    throw new HttpException(String.Format("Invalid button name: {0}", name));
            }
        }

        protected void SplitToolbarElement(string toolbarElement, out string elementType, out string elementName)
        {
            string[] elementItems = toolbarElement.Split('#');

            if (elementItems.Length > 1)
            {
                elementType = elementItems[0];
                elementName = elementItems[1];
            }
            else
            {
                elementType = "Button";
                elementName = elementItems[0];
            }
        }

        protected List<String> GetButtonNames()
        {
            List<String> buttonNames = new List<string>();

            string[] toolbars = Toolbars.Split(';');
            foreach (string toolbarElements in toolbars)
            {
                List<ToolstripData> toolstrips = ToolstripData.CreateToolstripData(toolbarElements);
                buttonNames.AddRange(ToolstripData.GetButtonNames(toolstrips));
            }

            return buttonNames;
        }

        protected void InsertToolstripDataItem(int index, List<ToolstripData> toolstrips, ToolstripData.ToolstripItemType toolstripItemType, string name)
        {
            if (toolstrips.Count == 0)
                toolstrips.Add(new ToolstripData());

            ToolstripData toolstripData = toolstrips[0];
            toolstripData.Items.Insert(index, new ToolstripData.ToolstripItemData(toolstripItemType, name));
        }
        
        protected void AddToggleModeButtons(List<ToolstripData> toolstripsBottomToolbar)
        {
            switch (this.ToggleMode)
            {
                case ToggleModeType.ToggleButton :
                case ToggleModeType.Buttons :
                    break;
                default :
                    return;
            }

            List<String> toolbarButtons = GetButtonNames();
            
            string designModeButtonName = this.DesignModeEditable ? "Design" : "View";
            bool hasDesignModeButton = toolbarButtons.Contains(designModeButtonName);
            bool hasHtmlModeButton = toolbarButtons.Contains("Html");
            int insertCount = 0;
            
            if (!hasHtmlModeButton)
            {
                InsertToolstripDataItem(0, toolstripsBottomToolbar, ToolstripData.ToolstripItemType.Button, "Html");
                insertCount++;
            }

            if (!hasDesignModeButton)
            {
                InsertToolstripDataItem(0, toolstripsBottomToolbar, ToolstripData.ToolstripItemType.Button, designModeButtonName);
                insertCount++;
            }

            if (insertCount > 0)
            {
                if (toolstripsBottomToolbar[0].Items.Count > insertCount)
                    InsertToolstripDataItem(insertCount, toolstripsBottomToolbar, ToolstripData.ToolstripItemType.Separator, String.Empty);
            }
        }
        
        protected void PackToolbarInfoList(List<ToolbarInfo> toolbarInfoList)
        {
            for (int i = toolbarInfoList.Count - 1; i >= 0; i--)
            {
                if (toolbarInfoList[i].Deleted) toolbarInfoList.RemoveAt(i);
            }
        }

        protected void FlushButtonInfo(ToolstripInfo toolstripInfo, ref List<ButtonInfo> buttons, ref int buttonsReference)
        {
            if (buttons.Count == 0) return;

            toolstripInfo.Items.Add(new ToolstripInfo.ToolstripItemInfo(ToolstripInfo.ElementType.Buttons, new ButtonBarInfo("Buttons" + buttonsReference.ToString(), buttons)));
            buttons = new List<ButtonInfo>();
            buttonsReference++;
        }
        
        protected ToolbarInfo CreateToolbarInformation(ToolstripData[] toolstrips, int row)
        {
            ToolbarInfo toolbarInfo = new ToolbarInfo("Toolbar" + row.ToString());
            int buttonsReference = 1;

            foreach (ToolstripData toolstripData in toolstrips)
            {
                ToolstripInfo toolstripInfo = new ToolstripInfo();

                string previousType = "None";
                List<ButtonInfo> buttons = new List<ButtonInfo>();

                for (int i = 0; i < toolstripData.Items.Count; i++)
                {
                    ToolstripData.ToolstripItemData item = toolstripData.Items[i];

                    switch (item.Type)
                    {
                        case ToolstripData.ToolstripItemType.Button:
                            buttons.Add(CreateButtonInfo(item.Name));
                            break;
                        case ToolstripData.ToolstripItemType.Select:
                            FlushButtonInfo(toolstripInfo, ref buttons, ref buttonsReference);
                            SelectInfo.SelectList selectList = CreateSelectList(item.Name);
                            toolstripInfo.Items.Add(new ToolstripInfo.ToolstripItemInfo(ToolstripInfo.ElementType.Select, new SelectInfo(this, item.Name, selectList, "Do" + item.Name, false)));
                            break;
                        case ToolstripData.ToolstripItemType.Separator:
                            FlushButtonInfo(toolstripInfo, ref buttons, ref buttonsReference);
                            toolstripInfo.Items.Add(new ToolstripInfo.ToolstripItemInfo(ToolstripInfo.ElementType.Separator, new SeparatorInfo()));
                            break;
                        default:
                            throw new HttpException(String.Format("Invalid toolbar element type: {0}", item.Type));
                    }

                    previousType = item.Type.ToString();
                }

                FlushButtonInfo(toolstripInfo, ref buttons, ref buttonsReference);
                toolbarInfo.Toolstrips.Add(toolstripInfo);
            }

            return toolbarInfo;
        }

        protected ToolbarInfo[] CreateToolbarInfoList()
        {
            List<ToolbarInfo> toolbarInfoList = new List<ToolbarInfo>();

            string[] toolbars = Toolbars.Length == 0 ? new string[] {} : Toolbars.Split(';');

            for (int i = 0; i < toolbars.Length; i++ )
            {
                bool lastToolbar = toolbars.Length == i + 1;

                List<ToolstripData> toolstrips = ToolstripData.CreateToolstripData(toolbars[i]);
                if (lastToolbar) AddToggleModeButtons(toolstrips);
                
                toolbarInfoList.Add(CreateToolbarInformation(toolstrips.ToArray(), i + 1));
            }

            OnCreateToolbarInfo(new CreateToolbarInfoEventArgs(this, toolbarInfoList));
            PackToolbarInfoList(toolbarInfoList);

            return toolbarInfoList.ToArray();
        }

        protected HtmlSelect CreateHtmlSelect(SelectInfo selectInfo, string previousType, string baseKey, ToolbarControlList toolbarControlList)
        {
            HtmlSelect htmlSelect = new HtmlSelect();
            toolbarControlList.Add(baseKey + "." + selectInfo.Name, htmlSelect);

            htmlSelect.Style["font-family"] = "Tahoma";
            htmlSelect.Style["font-size"] = "8pt";
            htmlSelect.Style["width"] = "100px";
            htmlSelect.Style["vertical-align"] = "middle";
            htmlSelect.Style["margin-left"] = "2px";
            htmlSelect.Style["display"] = IsToolbarElementHidden(selectInfo) ? "none" : "inline";

            foreach (string key in selectInfo.Items.Keys)
            {
                ListItem option = new ListItem(key, selectInfo.Items[key]);
                htmlSelect.Items.Add(option);
            }

            htmlSelect.Attributes["onchange"] = selectInfo.OnChange;

            return htmlSelect;
        }

        protected bool IsToolbarElementHidden(ToolbarElement toolbarElement)
        {
            if (toolbarElement.DisableMethod != ToolbarElement.DisableMethodType.Hide) return false;
            if (toolbarElement.Enabled == ToolbarElement.EnabledType.Never) return true;
            if (toolbarElement.Enabled == ToolbarElement.EnabledType.Always) return false;
            
            return toolbarElement.Enabled.ToString() != this.InitialMode.ToString();
        }
        
        protected HtmlGenericControl CreateButtonBar(ButtonBarInfo buttonBarInfo, string previousType, string baseKey, ToolbarControlList toolbarControlList)
        {
            HtmlGenericControl buttonBar = new HtmlGenericControl("div");
            buttonBar.Style["display"] = IsToolbarElementHidden(buttonBarInfo) ? "none" : "inline";

            baseKey += "." + buttonBarInfo.Name;
            toolbarControlList.Add(baseKey, buttonBar);

            bool ie6 = this.InternetExplorer && this.BrowserVersion < 7;
            string imgBorderStyle = ie6 ? "padding" : "border";
            string imgBorderValue = ie6 ? "1px" : "1px solid";

            foreach (ButtonInfo buttonInfo in buttonBarInfo.Buttons)
            {
                HtmlImage img = new HtmlImage();
                toolbarControlList.Add(baseKey + "." + buttonInfo.Name, img);
                
                img.Src = buttonInfo.ImageSrc;
                img.Alt = buttonInfo.ToolTip;
                img.Style["vertical-align"] = "middle";
                img.Style[imgBorderStyle] = imgBorderValue;
                img.Style["border-color"] = "transparent";
                img.Style["background-color"] = "transparent";
                img.Style["display"] = IsToolbarElementHidden(buttonInfo) ? "none" : "inline";
                img.Attributes["alt"] = buttonInfo.ToolTip;
                img.Attributes["title"] = buttonInfo.ToolTip;
                img.Attributes["onmouseover"] = buttonInfo.OnMouseOver;
                img.Attributes["onmouseout"] = buttonInfo.OnMouseOut;
                img.Attributes["onclick"] = buttonInfo.OnClick;

                buttonBar.Controls.Add(img);
            }

            switch (previousType)
            {
                case "Select" :
                    buttonBar.Style["margin-right"] = "8px";
                    break;
                default :
                    break;
            }
            
            return buttonBar;
        }

        protected HtmlGenericControl CreateSeparator()
        {
            HtmlGenericControl separator = new HtmlGenericControl("div");
            separator.Style["display"] = "inline";

            HtmlImage img = new HtmlImage();
            string resource;
            string imageSrc;

            bool ie6 = this.InternetExplorer && this.BrowserVersion < 7;
            string imgBorderStyle = ie6 ? "padding" : "border";
            string imgBorderValue = ie6 ? "1px" : "1px solid";

            switch (ToolstripBackgroundImage)
            {
                case ToolstripBackgroundImageType.Custom:
                    imageSrc = ToolstripBackgroundImageCustomPath + "/separator.gif";
                    break;
                default:
                    resource = "ParsElecom.HtmlEditor.Images.Toolstrips." + ToolstripBackgroundImage.ToString() + ".separator.gif";
                    imageSrc = GetEncodedWebResourceUrl(resource);
                    break;
            }

            if (NoToolstripBackgroundImage)
            {
                resource = "ParsElecom.HtmlEditor.Images.separator.gif";
                imageSrc = GetEncodedWebResourceUrl(resource);
            }

            img.Src = imageSrc;
            img.Attributes["alt"] = String.Empty;
            img.Style["vertical-align"] = "middle";
            img.Style[imgBorderStyle] = imgBorderValue;
            img.Style["border-color"] = "transparent";
            img.Style["background-color"] = "transparent";
            img.Style["display"] = "inline";

            separator.Controls.Add(img);

            return separator;
        }

        protected HtmlGenericControl CreatePadding(int height, int width)
        {
            HtmlGenericControl paddingBox = new HtmlGenericControl("div");
            paddingBox.Style["display"] = "inline";

            HtmlImage img = new HtmlImage();
            string resource = "ParsElecom.HtmlEditor.Images.pixel.gif";
            img.Src = GetEncodedWebResourceUrl(resource);
            img.Attributes["alt"] = String.Empty;
            img.Style["height"] = height.ToString() + "px";
            img.Style["width"] = width.ToString() + "px";
            img.Style["vertical-align"] = "middle";

            paddingBox.Controls.Add(img);

            return paddingBox;
        }

        protected Toolbar CreateToolbar(ToolbarInfo toolbarInfo, ToolbarControlList toolbarControlList, bool first, bool last)
        {
            Toolbar toolbar = new Toolbar(this);
            
            string key = toolbarInfo.Name;
            toolbarControlList.Add(key, toolbar);

            if (first)
            {
                toolbar.Style["position"] = "relative";
                toolbar.Style["right"] = "0px";
                toolbar.Style["top"] = "0px";
                //toolbar.Style["padding-top"] = ToolbarPadding.ToString() + "px";
                toolbar.Style["padding"] = "5px";

                if (this.ToolbarDocked)
                {
                    toolbar.Controls.Add(CreateRoundedCorner(true, true, ToBackColor));
                    toolbar.Controls.Add(CreateRoundedCorner(false, true, ToBackColor));
                }
            }
            else
            {
                toolbar.Style["margin-top"] = "1px";
            }

            if (last)
            {
                int paddingBottom = this.ToolbarDocked ? AdditionalToolbarPadding : ToolbarPadding;
                toolbar.Style["padding-bottom"] = paddingBottom.ToString() + "px";
            }

            toolbar.Style["display"] = IsToolbarElementHidden(toolbarInfo) ? "none" : "block";

            for (int i = 0; i < toolbarInfo.Toolstrips.Count; i++)
            {
                ToolstripInfo toolstripInfo = toolbarInfo.Toolstrips[i];
                
                Toolstrip toolStrip = new Toolstrip(this);
                int toolstripPadding = i == 0 ? ToolbarPadding : ToolstripPadding;
                toolStrip.Style["margin-right"] = toolstripPadding.ToString() + "px";
                toolStrip.Style["float"] = "right";

                toolbar.Controls.Add(toolStrip);

                toolStrip.AddToolbarElement(CreatePadding(ToolbarHeight, ToolstripPadding));

                string previousType = "None";

                foreach (ToolstripInfo.ToolstripItemInfo item in toolstripInfo.Items)
                {
                    switch (item.Type)
                    {
                        case ToolstripInfo.ElementType.Select:
                            SelectInfo selectInfo = (SelectInfo)item.Element;
                            toolStrip.AddToolbarElement(CreateHtmlSelect(selectInfo, previousType, key, toolbarControlList));
                            break;
                        case ToolstripInfo.ElementType.Buttons:
                            ButtonBarInfo buttonBarInfo = (ButtonBarInfo)item.Element;
                            toolStrip.AddToolbarElement(CreateButtonBar(buttonBarInfo, previousType, key, toolbarControlList));
                            break;
                        case ToolstripInfo.ElementType.Separator:
                            toolStrip.AddToolbarElement(CreateSeparator());
                            break;
                        default:
                            continue;
                    }

                    previousType = item.Type.ToString();
                }

                toolStrip.AddToolbarElement(CreatePadding(ToolbarHeight, ToolstripPadding));
            }

            toolbar.Controls.Add(new LiteralControl(Environment.NewLine));

            return toolbar;
        }

        protected HtmlGenericControl CreateToolbarContainer()
        {
            HtmlGenericControl toolbarContainer = new HtmlGenericControl("div");
            toolbarContainer.Style["background-color"] = ToToolbarColor;
            //toolbarContainer.Style["height"] = "30px";

            if (!ToolbarDocked)
            {
                toolbarContainer.Style["position"] = "absolute";

                if (ToolbarClass.Length > 0)
                    toolbarContainer.Attributes["class"] = ToolbarClass;
            }

            return toolbarContainer;
        }

        protected void CreateToolbars(ToolbarInfo[] toolbarInfoList)
        {
            this.toolbarContainer = CreateToolbarContainer();

            List<Toolbar> toolbars = new List<Toolbar>();
            ToolbarControlList toolbarControlList = new ToolbarControlList();

            for (int i = 0; i < toolbarInfoList.Length; i++ )
            {
                bool first = i == 0;
                bool last = toolbarInfoList.Length == i + 1;
                
                toolbars.Add(CreateToolbar(toolbarInfoList[i], toolbarControlList, first, last));
            }

            foreach (Toolbar toolbar in toolbars)
                this.toolbarContainer.Controls.Add(toolbar);

            this.Controls.Add(this.toolbarContainer);
            this.toolbarIDs = GetToolbarIDs(toolbars.ToArray());
            this.toolbarData = GetToolbarData(toolbarInfoList, toolbarControlList);
        }

        protected string GetToolbarData(ToolbarInfo[] toolbarInfoList, ToolbarControlList toolbarControlList)
        {
            string toolbarData = String.Empty;

            foreach (ToolbarInfo toolbarInfo in toolbarInfoList)
            {
                if (toolbarData.Length > 0) 
                    toolbarData += ";";
                
                toolbarData += toolbarInfo.GetToolbarData(toolbarControlList);
            }

            return toolbarData;
        }

        protected void CreateDialogData()
        {
            List<DialogInfo> dialogs = new List<DialogInfo>();

            string resource = "ParsElecom.HtmlEditor.Dialogs.Blank.htm";
            string frameUrl = GetWebResourceUrl(resource);
            dialogs.Add(new DialogInfo("Blank", frameUrl, " ", 250, 400));

            resource = "ParsElecom.HtmlEditor.Dialogs.Link.htm";
            frameUrl = GetWebResourceUrl(resource);
            dialogs.Add(new DialogInfo("Link", frameUrl, "افزودن لینک", 250, 400));

            resource = "ParsElecom.HtmlEditor.Dialogs.Image.htm";
            frameUrl = GetWebResourceUrl(resource);
            dialogs.Add(new DialogInfo("Image", frameUrl, "افزودن تصویر", 250, 400));

            resource = "ParsElecom.HtmlEditor.Dialogs.Color.htm";
            frameUrl = GetWebResourceUrl(resource);
            dialogs.Add(new DialogInfo("ForeColor", frameUrl, "رنگ متن", 250, 400));
            dialogs.Add(new DialogInfo("BackColor", frameUrl, "رنگ زمینه", 250, 400));

            OnCreateDialogInfo(new CreateDialogInfoEventArgs(this, dialogs));
            
            this.dialogData = GetDialogData(dialogs);
        }

        protected string GetDialogData(List<DialogInfo> dialogs)
        {
            string dialogData = String.Empty;

            foreach (DialogInfo dialog in dialogs)
            {
                if (dialogData.Length > 0) dialogData += ";";
                dialogData += dialog.GetDialogData();
            }

            return dialogData;
        }

        protected HtmlGenericControl CreateEditorArea(string id, out HtmlGenericControl editor, string editorID, string editorScrolling)
        {
            bool ie6 = this.InternetExplorer && this.BrowserVersion < 7;

            int frameBorderSize = EditorBorderSize > 1 ? 1 : 0;
            int areaBorderSize = EditorBorderSize - frameBorderSize;
            int areaWidth = EditorWidth + (frameBorderSize * 2);
            int areaHeight = EditorHeight + (frameBorderSize * 2);
            int editorWidth = ie6? EditorWidth - (frameBorderSize * 2) : EditorWidth;
            int editorHeight = ie6 ? EditorHeight - (frameBorderSize * 2) : EditorHeight;

            HtmlGenericControl area = new HtmlGenericControl("div");
            area.ID = id;
            area.Style["width"] = "100%";
            area.Style["height"] = areaHeight.ToString() + "px";
            area.Style["border"] = "solid " + areaBorderSize.ToString() + "px " + ToEditorBorderColor;
            area.Style["position"] = "relative";
            area.Style["right"] = "0px";
            area.Style["top"] = "0px";

            string frameTag = this.DesignMode ? "div" : "iframe";

            editor = new HtmlGenericControl(frameTag);
            editor.ID = editorID;
            editor.Style["background-color"] = ToEditorBackColor;
            editor.Style["color"] = ToEditorForeColor;
            editor.Style["position"] = "absolute";
            editor.Style["right"] = "0px";
            editor.Style["top"] = "0px";

            if (this.DesignMode)
            {
                editorWidth += frameBorderSize > 0 ? 2 : 0;
                editorHeight += frameBorderSize > 0 ? 2 : 0;
                editor.Style["width"] = "100%";
                editor.Style["height"] = editorHeight.ToString() + "px";
            }
            else
            {
                editor.Attributes["width"] = "100%";
                editor.Attributes["height"] = editorHeight.ToString() + "px";
                editor.Attributes["frameborder"] = "0";
                editor.Attributes["scrolling"] = editorScrolling;
            }
            
            if (frameBorderSize > 0)
                editor.Style["border"] = "solid " + frameBorderSize.ToString() + "px " + ToEditorInnerBorderColor;

            area.Controls.Add(editor);

            return area;
        }

        protected HtmlGenericControl CreateHtmlArea()
        {
            htmlArea = CreateEditorArea("contentText", out htmlEditor, "htmlEditor", "no");
            htmlArea.Style["display"] = this.InitialMode == Mode.Html ? "block" : "none";

            return htmlArea;
        }

        protected HtmlGenericControl CreateDesignArea()
        {
            designArea = CreateEditorArea("contentHtml", out designEditor, "designEditor", "auto");
            designArea.Style["display"] = this.InitialMode == Mode.Design ? "block" : "none";
            
            return designArea;
        }

        protected HtmlGenericControl CreateTab(int tabNumber, string name, string toolTip, string switchMode, string imgName, out HtmlImage img, out Label label)
        {
            int tabIndex = tabNumber - 1;
            int tabWidth = TabWidth;
            int tabSpacing = 2;
            int tabbarPadding = Math.Max(1, EditorBorderSize);
            int leftPosition = tabbarPadding + (tabWidth * tabIndex) + (tabSpacing * tabIndex);
            bool selected = this.InitialMode.ToString() == switchMode;
            
            HtmlGenericControl tab = new HtmlGenericControl("div");
            tab.Controls.Add(CreateRoundedCorner(true, false, ToBackColor));
            tab.Controls.Add(CreateRoundedCorner(false, false, ToBackColor));
            tab.Style["height"] = TabHeight.ToString() + "px";
            tab.Style["width"] = "100px";
            tab.Style["background-color"] = selected ? ToSelectedTabBackColor : ToTabBackColor;
            tab.Style["color"] = selected ? ToSelectedTabTextColor : ToTabForeColor;
            tab.Style["cursor"] = "pointer";
            tab.Style["position"] = "absolute";
            tab.Style["right"] = leftPosition.ToString() + "px";
            tab.Style["top"] = "0px";
            tab.Style["font-weight"] = selected ? "bold" : "normal";
            tab.Attributes["title"] = toolTip;
            tab.Attributes["onmouseover"] = GetSafeScriptMethod("TabOver(this)");
            tab.Attributes["onmouseout"] = GetSafeScriptMethod("TabOut(this)");
            tab.Attributes["onclick"] = GetSafeScriptMethod("SwitchMode(this, '" + switchMode + "')");

            img = new HtmlImage();
            string resource = "ParsElecom.HtmlEditor.Images." + imgName + ".gif";
            img.Src = GetEncodedWebResourceUrl(resource);
            img.Attributes["alt"] = this.DesignMode ? String.Empty : toolTip;
            img.Style["float"] = "right";

            tab.Controls.Add(img);

            label = new Label();
            label.Text = name;
            label.Style["float"] = "right";

            tab.Controls.Add(label);

            return tab;
        }

        protected HtmlGenericControl CreateTabbar()
        {
            HtmlGenericControl tabbar = new HtmlGenericControl("div");
            tabbar.Style["width"] = this.Width.ToString();
            tabbar.Style["height"] = TabbarHeight.ToString() + "px";
            tabbar.Style["font-family"] = "Tahoma";
            tabbar.Style["font-size"] = "8pt";
            tabbar.Style["background-color"] = ToTabbarBackColor;
            tabbar.Style["color"] = ToTabForeColor;
            tabbar.Style["position"] = "relative";
            tabbar.Style["right"] = "0px";
            tabbar.Style["top"] = "0px";
            tabbar.Style["display"] = this.ToggleMode == ToggleModeType.Tabs ? "block" : "none";

            int tabNumber = 0;
            
            HtmlImage designImg;
            Label designLabel;

            if (this.DesignModeEditable)
            {
                designTab = CreateTab(++tabNumber, "نوشتار", "نوشتار", "Design", "design", out designImg, out designLabel);
                designTab.ID = "designTab";
                designImg.Style["margin-right"] = this.DesignMode ? "0px" : "2px";
                designImg.Style["margin-top"] = this.DesignMode ? "0px" : "1px";
                designLabel.Style["margin-right"] = "2px";
                designLabel.Style["margin-top"] = "4px";

                tabbar.Controls.Add(designTab);
            }

            HtmlImage htmlImg;
            Label htmlLabel;
            string htmlToolTip = this.HtmlModeEditable ? "Html Mode" : "View Html";
            htmlTab = CreateTab(++tabNumber, "مشاهده کد", "مشاهده کد", "Html", "html", out htmlImg, out htmlLabel);
            htmlTab.ID = "htmlTab";
            htmlImg.Style["margin-right"] = this.DesignMode ? "0px" : "4px";
            htmlImg.Style["margin-top"] = this.DesignMode ? "0px" : "1px";
            htmlLabel.Style["margin-right"] = "2px";
            htmlLabel.Style["margin-top"] = "4px";

            tabbar.Controls.Add(htmlTab);

            if (!this.DesignModeEditable)
            {
                designTab = CreateTab(++tabNumber, "پیش نمایش", "پیش نمایش", "Design", "view", out designImg, out designLabel);
                designTab.ID = "viewTab";
                designImg.Style["margin-right"] = this.DesignMode ? "0px" : "4px";
                designImg.Style["margin-top"] = this.DesignMode ? "0px" : "1px";
                designLabel.Style["margin-right"] = "2px";
                designLabel.Style["margin-top"] = "4px";

                tabbar.Controls.Add(designTab);
            }

            return tabbar;
        }

        protected HtmlGenericControl CreateUpdateArea()
        {
            HtmlGenericControl updateArea = new HtmlGenericControl("div");
            updateArea.Style["display"] = "none";

            htmlencodedText = new HiddenField();
            htmlencodedText.ID = "htmlencodedText";
            htmlencodedText.Value = ClientEncode(this.htmlText);

            modifiedFlag = new HiddenField();
            modifiedFlag.ID = "Modified";
            modifiedFlag.Value = "false";
            
            saveButton = new Button();
            saveButton.ID = "SaveButton";
            saveButton.Click += new EventHandler(SaveButton_Click);
            saveButton.UseSubmitBehavior = false;

            updatePanel = new UpdatePanel();
            updatePanel.ID = "updatePanel";
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
            updatePanel.RenderMode = UpdatePanelRenderMode.Inline;

            updatePanel.ContentTemplateContainer.Controls.Add(htmlencodedText);
            updatePanel.ContentTemplateContainer.Controls.Add(modifiedFlag);
            updatePanel.ContentTemplateContainer.Controls.Add(saveButton);

            updateArea.Controls.Add(updatePanel);

            return updateArea;
        }

        protected void CreateAutoSaveValidators(ControlCollection controls)
        {
            this.validators = new List<CustomValidator>();

            String[] validationGroups = GetAutoSaveValidationGroups();
            string clientValidationFunction = FindScriptObject + ".AutoSave";

            foreach (string validationGroup in validationGroups)
            {
                CustomValidator autosaveValidator = new CustomValidator();
                autosaveValidator.ClientValidationFunction = clientValidationFunction;
                autosaveValidator.ValidationGroup = validationGroup;

                controls.Add(autosaveValidator);
                validators.Add(autosaveValidator);
            }
        }

        protected String[] GetAutoSaveValidationGroups()
        {
            if (ValidationGroups.Length > 0) return ValidationGroups;

            List<String> validationGroups = new List<string>();

            if (this.AutoSaveValidationGroups.Length == 0 || this.AutoSaveValidationGroups == ".")
            {
                validationGroups.Add(String.Empty);
            }
            else if (this.AutoSaveValidationGroups == "*")
            {
                validationGroups.Add(String.Empty);

                foreach (IValidator item in this.Page.Validators)
                {
                    BaseValidator validator = item as BaseValidator;
                    if (validator == null) continue;
                    if (validationGroups.Contains(validator.ValidationGroup)) continue;

                    validationGroups.Add(validator.ValidationGroup);
                }
            }
            else
            {
                String[] groupNames = GetStringList(this.AutoSaveValidationGroups);

                foreach (string groupName in groupNames)
                {
                    string name = groupName == "." ? String.Empty : groupName;
                    
                    if (!validationGroups.Contains(name))
                        validationGroups.Add(name);
                }
            }

            this.ValidationGroups = validationGroups.ToArray();

            return this.ValidationGroups;
        }
        
        void SaveButton_Click(object sender, EventArgs e)
        {
            OnSave(new SaveEventArgs(this));
        }

        protected void SetColorScheme()
        {
            ColorSchemeInfo colorSchemeInfo;
            
            switch (ColorScheme)
            {
                case ColorSchemeType.Custom :
                    colorSchemeInfo = new ColorSchemeInfo(this, ColorScheme.ToString());
                    colorSchemeInfo.ReadColors();
                    break;
                case ColorSchemeType.VisualStudio :
                    colorSchemeInfo = new VisualStudioColorSchemeInfo(this, ColorScheme.ToString());
                    break;
                case ColorSchemeType.PE:
                    colorSchemeInfo = new PEColorSchemeInfo(this, ColorScheme.ToString());
                    break;
                case ColorSchemeType.Default:
                    colorSchemeInfo = new ColorSchemeInfo(this, ColorScheme.ToString());
                    break;
                default:
                    return;
            }

            OnCreateColorSchemeInfo(new CreateColorSchemeInfoEventArgs(colorSchemeInfo));
            colorSchemeInfo.SetColors();
        }
        
        protected void CalculateDimensions(int numToolbars)
        {
            int w = (int)this.Width.Value;
            int h = (int)this.Height.Value;
            int tabbarHeight = this.ToggleMode == ToggleModeType.Tabs ? TabbarHeight : 0;
            int toolbarHeight = this.ToolbarDocked && numToolbars > 0 ? (ToolbarHeight * numToolbars) + (ToolbarPadding + AdditionalToolbarPadding) + (numToolbars - 1) : 0;
            EditorWidth = w - (EditorBorderSize * 2);
            EditorHeight = h - (EditorBorderSize * 2) - toolbarHeight - tabbarHeight;
        }

        protected override void CreateChildControls()
        {
            SetColorScheme();
            
            CreateDialogData();
            ToolbarInfo[] toolbarInfoList = CreateToolbarInfoList();

            CalculateDimensions(toolbarInfoList.Length);

            CreateDialogBox();
            CreateToolbars(toolbarInfoList);

            if (!this.DesignMode) this.Controls.Add(CreateHtmlArea());
            this.Controls.Add(CreateDesignArea());
            this.Controls.Add(CreateTabbar());
            this.Controls.Add(CreateUpdateArea());

            if (this.AutoSave)
                CreateAutoSaveValidators(this.Controls);

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            SaveButtonInfo.AttachButtons(this, GetStringList(this.SaveButtons));
            
            if (!this.DesignMode)
            {
                // Test for ScriptManager and register if it exists.
                sm = ScriptManager.GetCurrent(Page);

                if (sm == null)
                    throw new HttpException("A ScriptManager control must exist on the current page.");

                sm.RegisterScriptControl(this);
                
                // If updatePanel was manually updated, register a dataitem.
                if (sm.IsInAsyncPostBack && updateHtml)
                    sm.RegisterDataItem(htmlencodedText, "Updated");
            }

            base.OnPreRender(e);
        }

        protected string GetToolbarIDs(Toolbar[] toolbars)
        {
            string toolbarIDs = String.Empty;
            foreach (Toolbar toolbar in toolbars)
            {
                if (toolbarIDs.Length > 0) toolbarIDs += ",";
                toolbarIDs += toolbar.ClientID;
            }

            return toolbarIDs;
        }

        protected string GetDialogColors()
        {
            string dialogColors = ToDialogBackColor;
            dialogColors += ";" + ToDialogForeColor;
            dialogColors += ";" + ToDialogHeadingColor;
            dialogColors += ";" + ToDialogHeadingTextColor;
            dialogColors += ";" + ToDialogButtonBarColor;
            dialogColors += ";" + ToDialogBorderColor;
            dialogColors += ";" + ColorTranslator.ToHtml(this.DialogTableColor);
            dialogColors += ";" + ColorTranslator.ToHtml(this.DialogTabTextColor);
            dialogColors += ";" + ColorTranslator.ToHtml(this.DialogSelectedTabTextColor);
            dialogColors += ";" + ColorTranslator.ToHtml(this.DialogSelectedTabColor);
            dialogColors += ";" + ColorTranslator.ToHtml(this.DialogUnselectedTabColor);

            return dialogColors;
        }

        protected string GetDialogDimensions()
        {
            return this.DialogHeadingHeight.ToString() + ";" + this.DialogButtonBarHeight.ToString();
        }

        protected List<String> GetValidatorIDs()
        {
            List<String> validatorIDs = new List<string>();

            foreach (CustomValidator validator in this.validators)
            {
                validatorIDs.Add(validator.ClientID);
            }

            return validatorIDs;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
                sm.RegisterScriptDescriptors(this);

            base.Render(writer);
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference htmlEditorReference = new ScriptReference("ParsElecom.HtmlEditor.Scripts.HtmlEditor.js", "ParsElecom.HtmlEditor");
            ScriptReference dialogBehaviorReference = new ScriptReference("ParsElecom.HtmlEditor.Scripts.DialogBehavior.js", "ParsElecom.HtmlEditor");

            return new ScriptReference[] { htmlEditorReference, dialogBehaviorReference };
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("ParsElecom.HtmlEditor.HtmlEditor", this.ClientID);
            descriptor.AddProperty("htmlencodedTextID", this.htmlencodedText.ClientID);
            descriptor.AddProperty("modifiedID", this.modifiedFlag.ClientID);
            descriptor.AddProperty("htmlEditorID", this.htmlEditor.ClientID);
            descriptor.AddProperty("designEditorID", this.designEditor.ClientID);
            descriptor.AddProperty("htmlAreaID", this.htmlArea.ClientID);
            descriptor.AddProperty("designAreaID", this.designArea.ClientID);
            descriptor.AddProperty("htmlTabID", this.htmlTab.ClientID);
            descriptor.AddProperty("designTabID", this.designTab.ClientID);
            descriptor.AddProperty("dialogBoxIDs", this.dialogBoxIDs);
            descriptor.AddProperty("saveButtonID", this.saveButton.ClientID);
            descriptor.AddProperty("toolbarIDs", this.toolbarIDs);
            descriptor.AddProperty("editorForeColor", ColorTranslator.ToHtml(this.EditorForeColor));
            descriptor.AddProperty("editorBackColor", ColorTranslator.ToHtml(this.EditorBackColor));
            descriptor.AddProperty("toolbarColor", ColorTranslator.ToHtml(this.ToolbarColor));
            descriptor.AddProperty("tabForeColor", ColorTranslator.ToHtml(this.TabForeColor));
            descriptor.AddProperty("tabBackColor", ColorTranslator.ToHtml(this.TabBackColor));
            descriptor.AddProperty("selectedTabBackColor", ColorTranslator.ToHtml(this.SelectedTabBackColor));
            descriptor.AddProperty("selectedTabTextColor", ColorTranslator.ToHtml(this.SelectedTabTextColor));
            descriptor.AddProperty("tabMouseOverColor", ColorTranslator.ToHtml(this.TabMouseOverColor));
            descriptor.AddProperty("tabbarBackColor", ColorTranslator.ToHtml(this.TabbarBackColor));
            descriptor.AddProperty("buttonMouseOverColor", ColorTranslator.ToHtml(this.ButtonMouseOverColor));
            descriptor.AddProperty("buttonMouseOverBorderColor", ColorTranslator.ToHtml(this.ButtonMouseOverBorderColor));
            descriptor.AddProperty("dialogColors", this.GetDialogColors());
            descriptor.AddProperty("outputXHTML", this.OutputXHTML);
            descriptor.AddProperty("convertDeprecatedSyntax", this.ConvertDeprecatedSyntax);
            descriptor.AddProperty("convertParagraphs", this.ConvertParagraphs);
            descriptor.AddProperty("replaceNoBreakSpace", this.ReplaceNoBreakSpace);
            descriptor.AddProperty("showModifiedAsterick", this.ShowModifiedAsterick);
            descriptor.AddProperty("toolbarData", this.toolbarData);
            descriptor.AddProperty("dialogData", this.dialogData);
            descriptor.AddProperty("dialogDimensions", this.GetDialogDimensions());
            descriptor.AddProperty("pixelImageUrl", this.PixelImageUrl);
            descriptor.AddProperty("allowedTags", this.AllowedTags);
            descriptor.AddProperty("allowedAttributes", this.AllowedAttributes);
            descriptor.AddProperty("noScriptAttributes", this.NoScriptAttributes);
            descriptor.AddProperty("formatHtmlMode", this.FormatHtmlMode);
            descriptor.AddProperty("newLine", Environment.NewLine);
            descriptor.AddProperty("initialMode", this.InitialMode.ToString());
            descriptor.AddProperty("designModeEditable", this.DesignModeEditable);
            descriptor.AddProperty("htmlModeEditable", this.HtmlModeEditable);
            descriptor.AddProperty("designModeCss", this.GetDesignModeCss());
            descriptor.AddProperty("designModeEmulateIE7", this.DesignModeEmulateIE7);
            descriptor.AddProperty("textDirection", this.TextDirection.ToString());
            descriptor.AddProperty("version", typeof(ParsElecom.HtmlEditor.HtmlEditor).Assembly.GetName().Version.ToString());

            if (this.validators != null) descriptor.AddProperty("validatorIDs", this.GetValidatorIDs());

            if (!String.IsNullOrEmpty(this.ModifiedChanged)) descriptor.AddEvent("modifiedChanged", this.ModifiedChanged);
            if (!String.IsNullOrEmpty(this.ContextChanged)) descriptor.AddEvent("contextChanged", this.ContextChanged);

            return new ScriptDescriptor[] { descriptor };
        }

        #region IScriptControl Members

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            return GetScriptDescriptors();
        }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            return GetScriptReferences();
        }

        #endregion

        #region ColorSchemeInfo Classes

        public class ColorSchemeInfo
        {
            private HtmlEditor editor;
            private string name;

            public ToolstripBackgroundImageType ToolstripBackgroundImage = ToolstripBackgroundImageType.Default;
            public Color BackColor = ColorTranslator.FromHtml("White");
            public Color EditorForeColor = ColorTranslator.FromHtml("Black");
            public Color EditorBackColor = ColorTranslator.FromHtml("White");
            public Color EditorBorderColor = ColorTranslator.FromHtml("#7f9db9");
            public Color EditorInnerBorderColor = ColorTranslator.FromHtml("Gray");
            public Color ToolbarColor = ColorTranslator.FromHtml("#7f9db9");
            public Color TabForeColor = ColorTranslator.FromHtml("Black");
            public Color TabBackColor = ColorTranslator.FromHtml("LightSteelBlue");
            public Color SelectedTabBackColor = ColorTranslator.FromHtml("#7f9db9");
            public Color SelectedTabTextColor = ColorTranslator.FromHtml("White");
            public Color TabMouseOverColor = ColorTranslator.FromHtml("LightBlue");
            public Color TabbarBackColor = ColorTranslator.FromHtml("White");
            public Color ButtonMouseOverColor = ColorTranslator.FromHtml("#c1d2ee");
            public Color ButtonMouseOverBorderColor = ColorTranslator.FromHtml("#316ac5");
            public Color DialogBackColor = ColorTranslator.FromHtml("GhostWhite");
            public Color DialogForeColor = ColorTranslator.FromHtml("Black");
            public Color DialogHeadingColor = ColorTranslator.FromHtml("LightSteelBlue");
            public Color DialogHeadingTextColor = ColorTranslator.FromHtml("Black");
            public Color DialogButtonBarColor = ColorTranslator.FromHtml("LightSteelBlue");
            public Color DialogTableColor = ColorTranslator.FromHtml("#eeeeee");
            public Color DialogTabTextColor = ColorTranslator.FromHtml("Black");
            public Color DialogSelectedTabTextColor = ColorTranslator.FromHtml("White");
            public Color DialogUnselectedTabColor = ColorTranslator.FromHtml("LightSteelBlue");
            public Color DialogSelectedTabColor = ColorTranslator.FromHtml("#7f9db9");
            public Color DialogBorderColor = ColorTranslator.FromHtml("Black");

            public string Name
            {
                get { return this.name; }
            }

            public void ReadColors()
            {
                this.ToolstripBackgroundImage = editor.ToolstripBackgroundImage;

                this.BackColor = editor.BackColor;
                this.EditorForeColor = editor.EditorForeColor;
                this.EditorBackColor = editor.EditorBackColor;
                this.EditorBorderColor = editor.EditorBorderColor;
                this.EditorInnerBorderColor = editor.EditorInnerBorderColor;
                this.ToolbarColor = editor.ToolbarColor;
                this.TabForeColor = editor.TabForeColor;
                this.TabBackColor = editor.TabBackColor;
                this.SelectedTabBackColor = editor.SelectedTabBackColor;
                this.SelectedTabTextColor = editor.SelectedTabTextColor;
                this.TabMouseOverColor = editor.TabMouseOverColor;
                this.TabbarBackColor = editor.TabbarBackColor;
                this.ButtonMouseOverColor = editor.ButtonMouseOverColor;
                this.ButtonMouseOverBorderColor = editor.ButtonMouseOverBorderColor;
                this.DialogBackColor = editor.DialogBackColor;
                this.DialogForeColor = editor.DialogForeColor;
                this.DialogHeadingColor = editor.DialogHeadingColor;
                this.DialogHeadingTextColor = editor.DialogHeadingTextColor;
                this.DialogButtonBarColor = editor.DialogButtonBarColor;
                this.DialogTableColor = editor.DialogTableColor;
                this.DialogTabTextColor = editor.DialogTabTextColor;
                this.DialogSelectedTabTextColor = editor.DialogSelectedTabTextColor;
                this.DialogUnselectedTabColor = editor.DialogUnselectedTabColor;
                this.DialogSelectedTabColor = editor.DialogSelectedTabColor;
                this.DialogBorderColor = editor.DialogBorderColor;
            }

            public void SetColors()
            {
                editor.ToolstripBackgroundImage = this.ToolstripBackgroundImage;

                editor.BackColor = this.BackColor;
                editor.EditorForeColor = this.EditorForeColor;
                editor.EditorBackColor = this.EditorBackColor;
                editor.EditorBorderColor = this.EditorBorderColor;
                editor.EditorInnerBorderColor = this.EditorInnerBorderColor;
                editor.ToolbarColor = this.ToolbarColor;
                editor.TabForeColor = this.TabForeColor;
                editor.TabBackColor = this.TabBackColor;
                editor.SelectedTabBackColor = this.SelectedTabBackColor;
                editor.SelectedTabTextColor = this.SelectedTabTextColor;
                editor.TabMouseOverColor = this.TabMouseOverColor;
                editor.TabbarBackColor = this.TabbarBackColor;
                editor.ButtonMouseOverColor = this.ButtonMouseOverColor;
                editor.ButtonMouseOverBorderColor = this.ButtonMouseOverBorderColor;
                editor.DialogBackColor = this.DialogBackColor;
                editor.DialogForeColor = this.DialogForeColor;
                editor.DialogHeadingColor = this.DialogHeadingColor;
                editor.DialogHeadingTextColor = this.DialogHeadingTextColor;
                editor.DialogButtonBarColor = this.DialogButtonBarColor;
                editor.DialogTableColor = this.DialogTableColor;
                editor.DialogTabTextColor = this.DialogTabTextColor;
                editor.DialogSelectedTabTextColor = this.DialogSelectedTabTextColor;
                editor.DialogUnselectedTabColor = this.DialogUnselectedTabColor;
                editor.DialogSelectedTabColor = this.DialogSelectedTabColor;
                editor.DialogBorderColor = this.DialogBorderColor;
            }

            public Color GetColor(string htmlColor)
            {
                return ColorTranslator.FromHtml(htmlColor);
            }

            public ColorSchemeInfo(HtmlEditor editor, string name)
            {
                this.editor = editor;
                this.name = name;
            }
        }

        public class VisualStudioColorSchemeInfo : ColorSchemeInfo
        {
            public VisualStudioColorSchemeInfo(HtmlEditor editor, string name)
                : base(editor, name)
            {
                this.ToolstripBackgroundImage = ToolstripBackgroundImageType.VisualStudio;

                this.EditorBorderColor = GetColor("#dedcbc");
                this.ToolbarColor = GetColor("#dedcbc");
                this.TabBackColor = GetColor("#cccc99");
                this.SelectedTabBackColor = GetColor("#dedcbc");
                this.SelectedTabTextColor = GetColor("Black");
                this.TabMouseOverColor = GetColor("#d8d5a7");
                this.DialogHeadingColor = GetColor("#dedcbc");
                this.DialogHeadingTextColor = GetColor("Black");
                this.DialogButtonBarColor = GetColor("#dedcbc");
                this.DialogTabTextColor = GetColor("Black");
                this.DialogSelectedTabTextColor = GetColor("Black");
                this.DialogUnselectedTabColor = GetColor("#cccc99");
                this.DialogSelectedTabColor = GetColor("#dedcbc");
            }
        }

        public class PEColorSchemeInfo : ColorSchemeInfo
        {
            public PEColorSchemeInfo(HtmlEditor editor, string name)
                : base(editor, name)
            {
                this.ToolstripBackgroundImage = ToolstripBackgroundImageType.PE;

                this.EditorBorderColor = GetColor("#c4c4c4");
                this.ToolbarColor = GetColor("#e3e3e3");
                this.TabBackColor = GetColor("#f0f0f0");
                this.SelectedTabBackColor = GetColor("#14afca");
                this.SelectedTabTextColor = GetColor("white");
                this.TabMouseOverColor = GetColor("#14afca");
                this.DialogHeadingColor = GetColor("#e3e3e3");
                this.DialogHeadingTextColor = GetColor("Black");
                this.DialogButtonBarColor = GetColor("#e3e3e3");
                this.DialogTabTextColor = GetColor("Black");
                this.DialogSelectedTabTextColor = GetColor("white");
                this.DialogUnselectedTabColor = GetColor("#e3e3e3");
                this.DialogSelectedTabColor = GetColor("#14afca");
                this.TabbarBackColor = GetColor("#e3e3e3");
                this.ButtonMouseOverColor = GetColor("#14afca");
                this.ButtonMouseOverBorderColor = GetColor("#e3e3e3");
            }
        }

        #endregion

        #region CreateColorSchemeInfo EventHandler Classes

        public sealed class CreateColorSchemeInfoEventArgs : EventArgs
        {
            private ColorSchemeInfo colorSchemeInfo;

            public ColorSchemeInfo ColorScheme
            {
                get { return colorSchemeInfo; }
            }

            public CreateColorSchemeInfoEventArgs(ColorSchemeInfo colorSchemeInfo)
            {
                this.colorSchemeInfo = colorSchemeInfo;
            }
        }

        private static readonly object CreateColorSchemeInfoEventKey = new object();
        public delegate void CreateColorSchemeInfoEventHandler(object sender, CreateColorSchemeInfoEventArgs e);

        [Category("Appearance")]
        [Description("Event handler called when the color scheme is created.")]
        public event CreateColorSchemeInfoEventHandler CreateColorSchemeInfo
        {
            add
            {
                Events.AddHandler(CreateColorSchemeInfoEventKey, value);
            }
            remove
            {
                Events.RemoveHandler(CreateColorSchemeInfoEventKey, value);
            }
        }

        protected virtual void OnCreateColorSchemeInfo(CreateColorSchemeInfoEventArgs e)
        {
            CreateColorSchemeInfoEventHandler handler = (CreateColorSchemeInfoEventHandler)Events[CreateColorSchemeInfoEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Toolbar Classes

        public abstract class ToolbarElement
        {
            private string name;
            private EnabledType enabled;
            private DependencyType dependency;
            private DisableMethodType disableMethod;

            public enum EnabledType
            {
                Design,
                Html,
                Always,
                Never
            }

            public enum DependencyType
            {
                Editable,
                None
            }

            public enum DisableMethodType
            {
                Opacity,
                Hide
            }

            public string Name
            {
                get { return this.name; }
            }

            public EnabledType Enabled
            {
                get { return this.enabled; }
                set { this.enabled = value; }
            }

            public DependencyType Dependency
            {
                get { return this.dependency; }
                set { this.dependency = value; }
            }

            public DisableMethodType DisableMethod
            {
                get { return this.disableMethod; }
                set { this.disableMethod = value; }
            }

            public ToolbarElement(string name)
            {
                this.name = name;
                this.enabled = EnabledType.Always;
                this.dependency = DependencyType.None;
                this.disableMethod = DisableMethodType.Opacity;
            }
        }

        public class SelectInfo : ToolbarElement
        {
            private SelectList items;
            private string onChange;

            public class SelectList : Dictionary<String, String> {}

            public SelectList Items
            {
                get { return items; }
            }

            public string OnChange
            {
                get { return onChange; }
            }

            public SelectInfo(HtmlEditor editor, string name, SelectList items, string onChange, bool externalMethod) : base(name)
            {
                this.items = items;
                this.onChange = externalMethod ? onChange : editor.GetSafeScriptMethod(onChange + "(this.options[this.selectedIndex].value)");
                this.Enabled = EnabledType.Design;
                this.Dependency = DependencyType.Editable;
            }
        }

        public class ButtonInfo : ToolbarElement
        {
            private string imageSrc;
            private string toolTip;
            private string onMouseOver;
            private string onMouseOut;
            private string onClick;

            public string ImageSrc
            {
                get { return imageSrc; }
            }

            public string ToolTip
            {
                get { return toolTip; }
            }

            public string OnMouseOver
            {
                get { return onMouseOver; }
            }

            public string OnMouseOut
            {
                get { return onMouseOut; }
            }

            public string OnClick
            {
                get { return onClick; }
            }

            public ButtonInfo(HtmlEditor editor, string name, string toolTip)
                : base(name)
            {
                string resource = "ParsElecom.HtmlEditor.Images." + name.ToLowerInvariant() + ".gif";
                this.imageSrc = editor.GetEncodedWebResourceUrl(resource);
                this.toolTip = toolTip;
                this.onMouseOver = editor.GetSafeScriptMethod("MouseOver(this)");
                this.onMouseOut = editor.GetSafeScriptMethod("MouseOut(this)");

                bool hasDialog = editor.HasDialog(name);
                string method = hasDialog ? "DialogBox('" + name + "')" : "Do" + name + "()";
                this.onClick = editor.GetSafeScriptMethod(method);
                
                this.Enabled = EnabledType.Design;
                this.Dependency = DependencyType.Editable;
            }

            public ButtonInfo(HtmlEditor editor, string name, string imageSrc, string toolTip, string onClick, bool externalMethod)
                : base(name)
            {
                this.imageSrc = imageSrc;
                this.toolTip = toolTip;
                this.onMouseOver = editor.GetSafeScriptMethod("MouseOver(this)");
                this.onMouseOut = editor.GetSafeScriptMethod("MouseOut(this)");
                this.onClick = externalMethod ? onClick : editor.GetSafeScriptMethod(onClick);
            }
        }

        public class ButtonBarInfo : ToolbarElement
        {
            private List<ButtonInfo> buttons;

            public List<ButtonInfo> Buttons
            {
                get { return buttons; }
            }

            public ButtonBarInfo(string name, List<ButtonInfo> buttons)
                : base(name)
            {
                this.buttons = buttons;
            }
        }

        public class SeparatorInfo : ToolbarElement
        {
            public SeparatorInfo()
                : base("Separator")
            {
            }
        }

        public class ToolstripInfo : ToolbarElement
        {
            private List<ToolstripItemInfo> items = new List<ToolstripItemInfo>();

            public List<ToolstripItemInfo> Items
            {
                get { return this.items; }
            }

            public enum ElementType
            {
                Select,
                Buttons,
                Separator
            }

            public class ToolstripItemInfo
            {
                private ElementType type;
                private ToolbarElement element;

                public ElementType Type
                {
                    get { return type; }
                }

                public ToolbarElement Element
                {
                    get { return element; }
                }

                public ToolstripItemInfo(ElementType type, ToolbarElement element)
                {
                    this.type = type;
                    this.element = element;
                }
            }

            public ToolstripInfo()
                : base("Toolstrip")
            {
            }
        }

        public class ToolbarInfo : ToolbarElement
        {
            private List<ToolstripInfo> toolstrips = new List<ToolstripInfo>();
            private bool deleted = false;

            public List<ToolstripInfo> Toolstrips
            {
                get { return toolstrips; }
            }

            public bool Deleted
            {
                get { return deleted; }
            }

            public void Delete()
            {
                this.deleted = true;
            }

            public ToolbarElement FindElement(string name)
            {
                foreach (HtmlEditor.ToolstripInfo toolstripInfo in Toolstrips)
                {
                    foreach (HtmlEditor.ToolstripInfo.ToolstripItemInfo item in toolstripInfo.Items)
                    {
                        if (item.Element.Name == name) return item.Element;
                        if (item.Type == ToolstripInfo.ElementType.Buttons)
                        {
                            ButtonBarInfo buttonbarInfo = (ButtonBarInfo)item.Element;
                            foreach (ButtonInfo buttonInfo in buttonbarInfo.Buttons)
                            {
                                if (buttonInfo.Name == name) return buttonInfo;
                            }
                        }
                    }
                }

                return null;
            }

            public string GetToolbarData(ToolbarControlList toolbarControlList)
            {
                string key = Name;
                string toolbarData = GetToolbarElementData(this, key, toolbarControlList);
                string previousType = "None";

                foreach (ToolstripInfo toolstripInfo in Toolstrips)
                {
                    foreach (ToolstripInfo.ToolstripItemInfo item in toolstripInfo.Items)
                    {
                        switch (item.Type)
                        {
                            case ToolstripInfo.ElementType.Select :
                                SelectInfo selectInfo = (SelectInfo)item.Element;
                                toolbarData += GetSelectData(selectInfo, previousType, key, toolbarControlList);
                                previousType = "Select";
                                break;
                            case ToolstripInfo.ElementType.Buttons:
                                ButtonBarInfo buttonBarInfo = (ButtonBarInfo)item.Element;
                                toolbarData += GetButtonBarData(buttonBarInfo, previousType, key, toolbarControlList);
                                previousType = "Buttons";
                                break;
                            default :
                                continue;
                        }
                    }
                }
                
                return toolbarData;
            }

            protected string GetSelectData(SelectInfo selectInfo, string previousType, string baseKey, ToolbarControlList toolbarControlList)
            {
                string key = baseKey + "." + selectInfo.Name;
                string selectData = previousType == "None" ? "$" : "|";
                selectData += "Select?" + GetToolbarElementData(selectInfo, key, toolbarControlList);

                return selectData;
            }

            protected string GetButtonBarData(ButtonBarInfo buttonBarInfo, string previousType, string baseKey, ToolbarControlList toolbarControlList)
            {
                string key = baseKey + "." + buttonBarInfo.Name;
                string buttonBarData = previousType == "None" ? "$" : "|";
                buttonBarData += "Buttons?" + GetToolbarElementData(buttonBarInfo, key, toolbarControlList);
                
                string previousButtonBarType = "None";
                
                foreach (ButtonInfo buttonInfo in buttonBarInfo.Buttons)
                {
                    buttonBarData += GetButtonData(buttonInfo, previousButtonBarType, key, toolbarControlList);
                    previousButtonBarType = "Button";
                }

                return buttonBarData;
            }

            protected string GetButtonData(ButtonInfo buttonInfo, string previousButtonBarType, string baseKey, ToolbarControlList toolbarControlList)
            {
                string key = baseKey + "." + buttonInfo.Name;
                string buttonData = previousButtonBarType == "None" ? "=" : ",";
                buttonData += GetToolbarElementData(buttonInfo, key, toolbarControlList);

                return buttonData;
            }

            protected string GetToolbarElementData(ToolbarElement toolbarElement, string key, ToolbarControlList toolbarControlList)
            {
                return toolbarElement.Name + "@" + toolbarElement.Enabled.ToString() + "#" + toolbarElement.Dependency.ToString() + "#" + toolbarElement.DisableMethod.ToString() + "#" + toolbarControlList[key].ClientID;
            }

            public ToolbarInfo(string name) : base(name)
            {
            }
        }

        public class ToolstripData
        {
            private List<ToolstripItemData> items = new List<ToolstripItemData>();

            public enum ToolstripItemType
            {
                Separator,
                Button,
                Select
            }

            public List<ToolstripItemData> Items
            {
                get { return this.items; }
            }
            
            public class ToolstripItemData
            {
                private ToolstripItemType type;
                private string name;

                public ToolstripItemType Type
                {
                    get { return this.type; }
                }

                public string Name
                {
                    get { return this.name; }
                }

                public ToolstripItemData(ToolstripItemType type, string name)
                {
                    this.type = type;
                    this.name = name;
                }
            }

            public static List<String> GetButtonNames(List<ToolstripData> toolstripDataList)
            {
                List<String> buttonNames = new List<string>();

                foreach (ToolstripData toolstripData in toolstripDataList)
                {
                    foreach (ToolstripData.ToolstripItemData item in toolstripData.Items)
                    {
                        if (item.Type == ToolstripItemType.Button)
                        {
                            buttonNames.Add(item.Name);
                        }
                    }
                }

                return buttonNames;
            }
        
            public static List<ToolstripData> CreateToolstripData(string toolbarElements)
            {
                List<ToolstripData> toolstripDataList = new List<ToolstripData>();
                
                string[] toolstrips = toolbarElements.Split(':');

                foreach (string toolstrip in toolstrips)
                {
                    string strip = toolstrip.Trim();
                    if (strip.Length == 0) continue;

                    ToolstripData toolstripData = new ToolstripData();

                    string[] separators = strip.Split('|');

                    for (int i = 0; i < separators.Length; i++ )
                    {
                        string separateElements = separators[i].Trim();
                        if (separateElements.Length == 0) continue;

                        if (i > 0)
                            toolstripData.Items.Add(new ToolstripItemData(ToolstripItemType.Separator, String.Empty));

                        string[] elements = separateElements.Split(',');

                        foreach (string element in elements)
                        {
                            if (element.Trim().Length == 0) continue;

                            string[] elementInfo = element.Split('#');
                            string type = "Button";
                            string name = elementInfo[0].Trim();
                            
                            if (elementInfo.Length > 2)
                            {
                                throw new HttpException(String.Format("Invalid toolbar element: {0}", element));
                            }
                            else if (elementInfo.Length == 2)
                            {
                                type = name;
                                name = elementInfo[1].Trim();
                            }

                            ToolstripItemType toolstripItemType;

                            switch (type)
                            {
                                case "Button" :
                                    toolstripItemType = ToolstripItemType.Button;
                                    break;
                                case "Select" :
                                    toolstripItemType = ToolstripItemType.Select;
                                    break;
                                default :
                                    throw new HttpException(String.Format("Invalid toolbar element type: {0}", type));
                            }

                            toolstripData.Items.Add(new ToolstripItemData(toolstripItemType, name));
                        }
                    }

                    if (toolstripData.Items.Count > 0)
                        toolstripDataList.Add(toolstripData);
                }

                return toolstripDataList;
            }
        }

        public class Toolstrip : HtmlGenericControl
        {
            private HtmlGenericControl toolstripBody;
            
            protected enum SectionType
            {
                Begin,
                Body,
                End
            }

            public void AddToolbarElement(Control toolbarElement)
            {
                toolstripBody.Controls.Add(toolbarElement);
            }

            protected void SetBackgroundImage(HtmlEditor editor, HtmlGenericControl toolstripSection, SectionType sectionType, bool designerImplementation)
            {
                if (editor.NoToolstripBackgroundImage) return;
                
                string imageSrc;

                switch (editor.ToolstripBackgroundImage)
                {
                    case ToolstripBackgroundImageType.Custom:
                        imageSrc = editor.ToolstripBackgroundImageCustomPath + "/" + sectionType.ToString() + ".bmp";
                        break;
                    default :
                        string resource = "ParsElecom.HtmlEditor.Images.Toolstrips." + editor.ToolstripBackgroundImage.ToString() + "." + sectionType.ToString() + ".bmp";
                        imageSrc = editor.GetEncodedWebResourceUrl(resource);
                        break;
                }

                if (designerImplementation)
                {
                    HtmlImage img = new HtmlImage();
                    img.Src = imageSrc;
                    img.Style["height"] = editor.ToolbarHeight.ToString() + "px";
                    img.Style["width"] = "2px";
                    img.Style["vertical-align"] = "top";

                    toolstripSection.Controls.Add(img);
                }
                else
                {
                    toolstripSection.Style["background-image"] = "url(" + imageSrc + ")";
                    toolstripSection.Style["background-position"] = "left top";
                    toolstripSection.Style["background-repeat"] = sectionType == SectionType.Body ? "repeat-x" : "no-repeat";
                }
            }
            
            public Toolstrip(HtmlEditor editor)
                : base("div")
            {
                int height = editor.ToolbarHeight;
                
                this.Style["display"] = "inline";
                this.Style["height"] = height.ToString() + "px";
                this.Style["background-color"] = editor.ToToolbarColor;

                string floatStyle = editor.DesignMode ? "none" : "right";

                HtmlGenericControl toolstripBegin = new HtmlGenericControl("div");
                toolstripBegin.Style["float"] = floatStyle;
                toolstripBegin.Style["display"] = "inline";
                toolstripBegin.Style["width"] = "2px";
                toolstripBegin.Style["height"] = height.ToString() + "px";
                SetBackgroundImage(editor, toolstripBegin, SectionType.Begin, editor.DesignMode);

                toolstripBody = new HtmlGenericControl("div");
                toolstripBody.Style["float"] = floatStyle;
                toolstripBody.Style["display"] = "inline";
                toolstripBody.Style["height"] = height.ToString() + "px";
                SetBackgroundImage(editor, toolstripBody, SectionType.Body, false);

                HtmlGenericControl toolstripEnd = new HtmlGenericControl("div");
                toolstripEnd.Style["float"] = floatStyle;
                toolstripEnd.Style["display"] = "inline";
                toolstripEnd.Style["width"] = "2px";
                toolstripEnd.Style["height"] = height.ToString() + "px";
                SetBackgroundImage(editor, toolstripEnd, SectionType.End, editor.DesignMode);

                this.Controls.Add(toolstripBegin);
                this.Controls.Add(toolstripBody);
                this.Controls.Add(toolstripEnd);
            }
        }

        public class Toolbar : HtmlGenericControl
        {
            public Toolbar(HtmlEditor editor)
                : base("div")
            {
                if (editor.ToolbarDocked)
                    this.Style["width"] = editor.Width.ToString();

                this.Style["height"] = editor.ToolbarHeight.ToString() + "px";
                this.Style["background-color"] = editor.ToToolbarColor;
            }
        }

        public class ToolbarControlList : Dictionary<String, Control> {}

        #endregion

        #region CreateToolbarInfo EventHandler Classes

        public sealed class CreateToolbarInfoEventArgs : EventArgs
        {
            private HtmlEditor editor;
            private List<ToolbarInfo> toolbarInfoList;

            public List<ToolbarInfo> ToolbarInfoList
            {
                get { return toolbarInfoList; }
            }

            private ButtonInfo FindButtonInfo(List<ButtonInfo> buttonList, string name)
            {
                foreach (ButtonInfo buttonInfo in buttonList)
                {
                    if (buttonInfo.Name == name) return buttonInfo;
                }

                return null;
            }

            public void SetButtons(List<ButtonInfo> buttonList, string commaDelimitedButtonNames)
            {
                SetButtons(buttonList, editor.GetStringList(commaDelimitedButtonNames));
            }

            private void SetButtons(List<ButtonInfo> buttonList, string[] names)
            {
                List<ButtonInfo> newList = new List<ButtonInfo>();

                foreach (string name in names)
                {
                    ButtonInfo buttonInfo = FindButtonInfo(buttonList, name);
                    if (buttonInfo == null)
                        throw new HttpException(String.Format("No button named '{0}' was found in the button list", name));

                    newList.Add(buttonInfo);
                }

                buttonList.Clear();
                buttonList.AddRange(newList);
            }

            public void AddButton(List<ButtonInfo> buttonList, ButtonInfo buttonInfo)
            {
                buttonList.Add(buttonInfo);
            }

            public CreateToolbarInfoEventArgs(HtmlEditor editor, List<ToolbarInfo> toolbarInfoList)
            {
                this.editor = editor;
                this.toolbarInfoList = toolbarInfoList;
            }
        }

        private static readonly object CreateToolbarInfoEventKey = new object();
        public delegate void CreateToolbarInfoEventHandler(object sender, CreateToolbarInfoEventArgs e);

        [Category("Behavior")]
        [Description("Event handler called when toolbars are created.")]
        public event CreateToolbarInfoEventHandler CreateToolbarInfo
        {
            add
            {
                Events.AddHandler(CreateToolbarInfoEventKey, value);
            }
            remove
            {
                Events.RemoveHandler(CreateToolbarInfoEventKey, value);
            }
        }

        protected virtual void OnCreateToolbarInfo(CreateToolbarInfoEventArgs e)
        {
            CreateToolbarInfoEventHandler handler = (CreateToolbarInfoEventHandler)Events[CreateToolbarInfoEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region DialogInfo Classes

        public class DialogInfo
        {
            private string name;
            private string frameUrl;
            private string heading;
            private int height;
            private int width;

            public string Name
            {
                get { return this.name; }
            }

            public string FrameUrl
            {
                get { return this.frameUrl; }
            }

            public string Heading
            {
                get { return this.heading; }
            }

            public int Height
            {
                get { return this.height; }
            }

            public int Width
            {
                get { return this.width; }
            }

            public DialogInfo(string name, string frameUrl, string heading, int height, int width)
            {
                if (heading.IndexOf(",") >= 0)
                    throw new HttpException(String.Format("Dialog Dialog heading cannot contain a comma (Name = {0})", name));
                
                this.name = name;
                this.frameUrl = frameUrl;
                this.heading = heading;
                this.height = height;
                this.width = width;
            }

            public string GetDialogData()
            {
                string dialogData = String.Empty;

                dialogData += name;
                dialogData += "," + frameUrl;
                dialogData += "," + heading;
                dialogData += "," + height.ToString();
                dialogData += "," + width.ToString();

                return dialogData;
            }
        }

        #endregion

        #region CreateDialogInfo EventHandler Classes

        public sealed class CreateDialogInfoEventArgs : EventArgs
        {
            private HtmlEditor editor;
            private List<DialogInfo> dialogs;

            public List<DialogInfo> Dialogs
            {
                get { return dialogs; }
            }

            public CreateDialogInfoEventArgs(HtmlEditor editor, List<DialogInfo> dialogs)
            {
                this.editor = editor;
                this.dialogs = dialogs;
            }
        }

        private static readonly object CreateDialogInfoEventKey = new object();
        public delegate void CreateDialogInfoEventHandler(object sender, CreateDialogInfoEventArgs e);

        [Category("Behavior")]
        [Description("Event handler called when dialogs are created.")]
        public event CreateDialogInfoEventHandler CreateDialogInfo
        {
            add
            {
                Events.AddHandler(CreateDialogInfoEventKey, value);
            }
            remove
            {
                Events.RemoveHandler(CreateDialogInfoEventKey, value);
            }
        }

        protected virtual void OnCreateDialogInfo(CreateDialogInfoEventArgs e)
        {
            CreateDialogInfoEventHandler handler = (CreateDialogInfoEventHandler)Events[CreateDialogInfoEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Save EventHandler Classes

        public sealed class SaveEventArgs : EventArgs
        {
            private HtmlEditor editor;

            public string Text
            {
                get { return editor.Text; }
            }

            public SaveEventArgs(HtmlEditor editor)
            {
                this.editor = editor;
            }
        }

        private static readonly object SaveEventKey = new object();
        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        [Category("Behavior")]
        [Description("Event handler called when the Save toolbar button is clicked.")]
        public event SaveEventHandler Save
        {
            add
            {
                Events.AddHandler(SaveEventKey, value);
            }
            remove
            {
                Events.RemoveHandler(SaveEventKey, value);
            }
        }

        protected virtual void OnSave(SaveEventArgs e)
        {
            SaveEventHandler handler = (SaveEventHandler)Events[SaveEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region SaveButtonInfo Classes

        public class SaveButtonInfo
        {
            HtmlEditor editor;
            string[] pathElements;
            bool absolute;

            public string ControlPath
            {
                get
                {
                    string controlPath = String.Empty;

                    foreach (string pathElement in pathElements)
                    {
                        if (controlPath.Length > 0)
                            controlPath += "/";

                        controlPath += pathElement;
                    }

                    return controlPath;
                }
            }

            public SaveButtonInfo(HtmlEditor editor, string controlPath)
            {
                this.editor = editor;
                this.pathElements = controlPath.Split('/');
                this.absolute = pathElements.Length > 1 && pathElements[0] == "~";
            }

            public bool Attach()
            {
                Control control = FindControl();
                if (control == null) return false;

                AttachClientSave(this.editor, control);

                return true;
            }

            public static void AttachButtons(HtmlEditor editor, string[] controlPaths)
            {
                SaveButtonInfoList buttons = new SaveButtonInfoList(editor, controlPaths);
                buttons.Attach();
            }

            public static void AttachButton(HtmlEditor editor, Control control)
            {
                SaveButtonInfo.AttachClientSave(editor, control);
            }

            protected Control FindNearest(string controlID)
            {
                Control container = this.editor.NamingContainer;

                while (container != null)
                {
                    Control control = container.FindControl(controlID);
                    if (control != null) return control;

                    container = container.NamingContainer;
                }

                return null;
            }

            protected Control FindExact(Control container, int index)
            {
                Control control = null;

                for (int i = index; i < pathElements.Length; i++)
                {
                    control = container.FindControl(this.pathElements[i]);
                    if (control == null) return null;

                    container = control;
                }

                return control;
            }

            protected Control FindStartContainer(out int index)
            {
                index = 0;
                Control container = this.editor.NamingContainer;

                for (int i = 0; i < pathElements.Length - 1; i++)
                {
                    if (pathElements[i] == "..")
                    {
                        index = i + 1;

                        container = container.NamingContainer;
                        if (container == null) return null;
                    }
                    else
                    {
                        index = i;
                        break;
                    }
                }

                return container;
            }

            protected Control FindControl()
            {
                if (pathElements.Length == 1)
                {
                    return FindNearest(pathElements[0]);
                }
                else if (this.absolute)
                {
                    FindExact(this.editor.Page, 1);
                }
                else
                {
                    int index;
                    Control container = FindStartContainer(out index);
                    if (container == null) return null;

                    return FindExact(container, index);
                }

                return null;
            }

            protected static string GetModifiedOnClientClick(HtmlEditor editor, string onclientClick)
            {
                string editorSave = editor.FindScriptObject + ".Save()";
                if (onclientClick.Length == 0) return editorSave;

                if (onclientClick.IndexOf(editorSave) >= 0)
                    return onclientClick;

                return editorSave + "; " + onclientClick;
            }

            protected static void AttachClientSave(HtmlEditor editor, Control control)
            {
                if (control is Button)
                {
                    Button b = (Button)control;
                    b.OnClientClick = GetModifiedOnClientClick(editor, b.OnClientClick);
                }
                else if (control is LinkButton)
                {
                    LinkButton lb = (LinkButton)control;
                    lb.OnClientClick = GetModifiedOnClientClick(editor, lb.OnClientClick);
                }
                else if (control is ImageButton)
                {
                    ImageButton ib = (ImageButton)control;
                    ib.OnClientClick = GetModifiedOnClientClick(editor, ib.OnClientClick);
                }
                else
                {
                    throw new HttpException(String.Format("Invalid SaveButton control type: {0}", control.ID));
                }
            }
        }

        public class SaveButtonInfoList
        {
            private List<SaveButtonInfo> buttons;

            public SaveButtonInfoList(HtmlEditor editor, string[] controlPaths)
            {
                this.buttons = new List<SaveButtonInfo>();

                foreach (string controlPath in controlPaths)
                {
                    this.buttons.Add(new SaveButtonInfo(editor, controlPath));
                }
            }

            public void Attach()
            {
                foreach (SaveButtonInfo button in buttons)
                {
                    if (!button.Attach())
                        throw new HttpException(String.Format("Invalid SaveButton control path: {0}", button.ControlPath));
                }
            }
        }

        #endregion
    }
}
