﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using HandyControl.Controls;
using HandyControl.Data;

namespace HandyControl.Tools
{
    public class ButtonGroupItemStyleSelector : StyleSelector
    {
        private static readonly Dictionary<string, Style> StyleDict = new()
        {
            [ResourceToken.RadioGroupItemSingle] = ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemSingle),
            [ResourceToken.RadioGroupItemHorizontalFirst] = ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemHorizontalFirst),
            [ResourceToken.RadioGroupItemHorizontalLast] = ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemHorizontalLast),
            [ResourceToken.RadioGroupItemVerticalFirst] = ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemVerticalFirst),
            [ResourceToken.RadioGroupItemVerticalLast] = ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemVerticalLast),
            [ResourceToken.RadioGroupItemDefault] = ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemDefault),

            [ResourceToken.ButtonGroupItemSingle] = ResourceHelper.GetResource<Style>(ResourceToken.ButtonGroupItemSingle),
            [ResourceToken.ButtonGroupItemHorizontalFirst] = ResourceHelper.GetResource<Style>(ResourceToken.ButtonGroupItemHorizontalFirst),
            [ResourceToken.ButtonGroupItemHorizontalLast] = ResourceHelper.GetResource<Style>(ResourceToken.ButtonGroupItemHorizontalLast),
            [ResourceToken.ButtonGroupItemVerticalFirst] = ResourceHelper.GetResource<Style>(ResourceToken.ButtonGroupItemVerticalFirst),
            [ResourceToken.ButtonGroupItemVerticalLast] = ResourceHelper.GetResource<Style>(ResourceToken.ButtonGroupItemVerticalLast),
            [ResourceToken.ButtonGroupItemDefault] = ResourceHelper.GetResource<Style>(ResourceToken.ButtonGroupItemDefault),

            [ResourceToken.ToggleButtonGroupItemSingle] = ResourceHelper.GetResource<Style>(ResourceToken.ToggleButtonGroupItemSingle),
            [ResourceToken.ToggleButtonGroupItemHorizontalFirst] = ResourceHelper.GetResource<Style>(ResourceToken.ToggleButtonGroupItemHorizontalFirst),
            [ResourceToken.ToggleButtonGroupItemHorizontalLast] = ResourceHelper.GetResource<Style>(ResourceToken.ToggleButtonGroupItemHorizontalLast),
            [ResourceToken.ToggleButtonGroupItemVerticalFirst] = ResourceHelper.GetResource<Style>(ResourceToken.ToggleButtonGroupItemVerticalFirst),
            [ResourceToken.ToggleButtonGroupItemVerticalLast] = ResourceHelper.GetResource<Style>(ResourceToken.ToggleButtonGroupItemVerticalLast),
            [ResourceToken.ToggleButtonGroupItemDefault] = ResourceHelper.GetResource<Style>(ResourceToken.ToggleButtonGroupItemDefault)
        };

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (container is ButtonGroup buttonGroup && item is ButtonBase buttonBase)
            {
                var count = buttonGroup.Items.Count;

                switch (buttonBase)
                {
                    case RadioButton:
                        {
                            if (count == 1)
                            {
                                return StyleDict[ResourceToken.RadioGroupItemSingle];
                            }

                            var index = buttonGroup.Items.IndexOf(buttonBase);
                            return buttonGroup.Orientation == Orientation.Horizontal
                                ? index == 0
                                    ? StyleDict[ResourceToken.RadioGroupItemHorizontalFirst]
                                    : StyleDict[index == count - 1
                                        ? ResourceToken.RadioGroupItemHorizontalLast
                                        : ResourceToken.RadioGroupItemDefault]
                                : index == 0
                                    ? StyleDict[ResourceToken.RadioGroupItemVerticalFirst]
                                    : StyleDict[index == count - 1
                                        ? ResourceToken.RadioGroupItemVerticalLast
                                        : ResourceToken.RadioGroupItemDefault];
                        }

                    case Button:
                        {
                            if (count == 1)
                            {
                                return StyleDict[ResourceToken.ButtonGroupItemSingle];
                            }

                            var index = buttonGroup.Items.IndexOf(buttonBase);
                            return buttonGroup.Orientation == Orientation.Horizontal
                                ? index == 0
                                    ? StyleDict[ResourceToken.ButtonGroupItemHorizontalFirst]
                                    : StyleDict[index == count - 1
                                        ? ResourceToken.ButtonGroupItemHorizontalLast
                                        : ResourceToken.ButtonGroupItemDefault]
                                : index == 0
                                    ? StyleDict[ResourceToken.ButtonGroupItemVerticalFirst]
                                    : StyleDict[index == count - 1
                                        ? ResourceToken.ButtonGroupItemVerticalLast
                                        : ResourceToken.ButtonGroupItemDefault];
                        }

                    case ToggleButton:
                        {
                            if (count == 1)
                            {
                                return StyleDict[ResourceToken.ToggleButtonGroupItemSingle];
                            }

                            var index = buttonGroup.Items.IndexOf(buttonBase);
                            return buttonGroup.Orientation == Orientation.Horizontal
                                ? index == 0
                                    ? StyleDict[ResourceToken.ToggleButtonGroupItemHorizontalFirst]
                                    : StyleDict[index == count - 1
                                        ? ResourceToken.ToggleButtonGroupItemHorizontalLast
                                        : ResourceToken.ToggleButtonGroupItemDefault]
                                : index == 0
                                    ? StyleDict[ResourceToken.ToggleButtonGroupItemVerticalFirst]
                                    : StyleDict[index == count - 1
                                        ? ResourceToken.ToggleButtonGroupItemVerticalLast
                                        : ResourceToken.ToggleButtonGroupItemDefault];
                        }
                }
            }

            return null;
        }
    }
}
