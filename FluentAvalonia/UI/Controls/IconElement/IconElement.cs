﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.ComponentModel;
using System.Globalization;

namespace FluentAvalonia.UI.Controls
{
    [TypeConverter(typeof(IconElementConverter))]
    public class IconElement : Control
    {
        static IconElement()
        {
            AffectsRender<IconElement>(ForegroundProperty);
        }

        public static readonly AttachedProperty<IBrush> ForegroundProperty =
            TextBlock.ForegroundProperty.AddOwner<IconElement>();

        public IBrush Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
    }

    public class IconElementConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Symbol) || sourceType == typeof(IconSource))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is Symbol symbol)
            {
                return new SymbolIcon { Symbol = symbol };
            }
            else if (value is IconSource ico)
            {
                if (ico is FontIconSource fis)
                {
                    return IconHelpers.CreateFontIconFromFontIconSource(fis);
                }
                else if (ico is SymbolIconSource sis)
                {
                    return IconHelpers.CreateSymbolIconFromSymbolIconSource(sis);
                }
                else if (ico is PathIconSource pis)
                {
                    return IconHelpers.CreatePathIconFromPathIconSource(pis);
                }
                else if (ico is BitmapIconSource bis)
                {
                    return IconHelpers.CreateBitmapIconFromBitmapIconSource(bis);
                }
            }
            else if (value is string val)
            {
                //First we try if the text is a valid Symbol
                if (Enum.TryParse(typeof(Symbol), val, out object sym))
                {
                    return new SymbolIcon() { Symbol = (Symbol)sym };
                }

                //Try a PathIcon
                if (PathIcon.IsDataValid(val, out Geometry g))
                {
                    return new PathIcon() { Data = g };
                }

                try
                {
                    if (Uri.TryCreate(val, UriKind.RelativeOrAbsolute, out Uri result))
                    {
                        return new BitmapIcon() { UriSource = result };
                    }
                }
                catch { }

                //If we've reached this point, we'll make a FontIcon
                //Glyph can be anything (sort of), so we don't need to Try/Catch
                return new FontIconSource() { Glyph = val };

            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
