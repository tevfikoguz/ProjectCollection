﻿using Dreambuild.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BubbleFlow
{
    public static class ViewerToolManager
    {
        private static ViewerTool _exclusiveTool;
        public static ViewerTool ExclusiveTool
        {
            get
            {
                return _exclusiveTool;
            }
            set
            {
                if (_exclusiveTool != null)
                {
                    _exclusiveTool.ExitToolHandler();
                }
                _exclusiveTool = value;
                _exclusiveTool.EnterToolHandler();
            }
        }

        private static List<ViewerTool> _overlayTools = new List<ViewerTool>();
        public static ReadOnlyCollection<ViewerTool> OverlayTools
        {
            get
            {
                return _overlayTools.AsReadOnly();
            }
        }

        public static IEnumerable<ViewerTool> Tools
        {
            get
            {
                if (ExclusiveTool != null)
                {
                    yield return ExclusiveTool;
                }
                foreach (var tool in OverlayTools)
                {
                    yield return tool;
                }
            }
        }

        public static void AddTool(ViewerTool tool)
        {
            tool.EnterToolHandler();
            _overlayTools.Add(tool);
        }

        public static void RemoveTool(ViewerTool tool)
        {
            if (_overlayTools.Remove(tool))
            {
                tool.ExitToolHandler();
            }
        }

        public static void ClearTools()
        {
            _overlayTools.ForEach(x => x.ExitToolHandler());
            _overlayTools.Clear();
        }

        public static void SetFrameworkElement(FrameworkElement element) // mod 20180629
        {
            element.MouseMove += (s, args) => Tools.ForEach(t => t.MouseMoveHandler(s, args));
            element.MouseDown += (s, args) =>
            {
                if (args.ClickCount >= 2)
                {
                    Tools.ForEach(t => t.MouseDoubleClickHandler(s, args));
                }
                else
                {
                    Tools.ForEach(t => t.MouseDownHandler(s, args));
                }
            };
            element.MouseUp += (s, args) => Tools.ForEach(t => t.MouseUpHandler(s, args));
            element.MouseWheel += (s, args) => Tools.ForEach(t => t.MouseWheelHandler(s, args));
            element.KeyDown += (s, args) => Tools.ForEach(t => t.KeyDownHandler(s, args));
        }
    }

    public abstract class ViewerTool
    {
        public virtual IEnumerable<UIElement> CanvasElements { get { yield break; } } // mod 20140707
        public virtual IEnumerable<UIElement> WorldElements { get { yield break; } } // newly 20140707
        public virtual ContextMenu ContextMenu { get { return null; } } // newly 20140623

        public virtual void Render()
        {
        }

        public virtual void MouseMoveHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
        }

        public virtual void KeyDownHandler(object sender, KeyEventArgs e)
        {
        }

        public virtual void EnterToolHandler()
        {
            //TempElements.ForEach(x => MapControl.Current.Children.Add(x));
        }

        public virtual void ExitToolHandler()
        {
            //TempElements.ForEach(x => MapControl.Current.Children.Remove(x));
        }
    }

    public class CombinedViewerTool : ViewerTool
    {
        private ViewerTool[] _tools;

        public CombinedViewerTool(params ViewerTool[] tools)
        {
            _tools = tools;
        }

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                return _tools.SelectMany(x => x.CanvasElements);
            }
        }

        public override IEnumerable<UIElement> WorldElements
        {
            get
            {
                return _tools.SelectMany(x => x.WorldElements);
            }
        }

        public override void Render()
        {
            _tools.ForEach(x => x.Render());
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            _tools.ForEach(x => x.MouseMoveHandler(sender, e));
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseDownHandler(sender, e));
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseUpHandler(sender, e));
        }

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseDoubleClickHandler(sender, e));
        }

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            _tools.ForEach(x => x.MouseWheelHandler(sender, e));
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            _tools.ForEach(x => x.KeyDownHandler(sender, e));
        }

        public override void EnterToolHandler()
        {
            _tools.ForEach(x => x.EnterToolHandler());
        }

        public override void ExitToolHandler()
        {
            _tools.ForEach(x => x.ExitToolHandler());
        }
    }
}