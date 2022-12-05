using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using HandyControl.Properties.Langs;

namespace HandyControl.Controls;

public class PropertyResolver
{
    public virtual string ResolveCategory(PropertyDescriptor propertyDescriptor)
    {
        var categoryAttribute = propertyDescriptor.Attributes.OfType<CategoryAttribute>().FirstOrDefault();

        return categoryAttribute == null ?
            Lang.Miscellaneous :
            string.IsNullOrEmpty(categoryAttribute.Category) ?
                Lang.Miscellaneous :
                categoryAttribute.Category;
    }

    public virtual string ResolveDisplayName(PropertyDescriptor propertyDescriptor)
    {
        var displayName = propertyDescriptor.DisplayName;
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = propertyDescriptor.Name;
        }

        return displayName;
    }

    public virtual string ResolveDescription(PropertyDescriptor propertyDescriptor) => propertyDescriptor.Description;

    public virtual bool ResolveIsBrowsable(PropertyDescriptor propertyDescriptor) => propertyDescriptor.IsBrowsable;

    public virtual bool ResolveIsDisplay(PropertyDescriptor propertyDescriptor) => propertyDescriptor.IsLocalizable;

    public virtual bool ResolveIsReadOnly(PropertyDescriptor propertyDescriptor) => propertyDescriptor.IsReadOnly;

    public virtual object ResolveDefaultValue(PropertyDescriptor propertyDescriptor)
    {
        var defaultValueAttribute = propertyDescriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
        return defaultValueAttribute?.Value;
    }

    public PropertyEditorBase ResolveEditor(PropertyDescriptor propertyDescriptor)
    {
        var editorAttribute = propertyDescriptor.Attributes.OfType<EditorAttribute>().FirstOrDefault();
        var editor = editorAttribute == null || string.IsNullOrEmpty(editorAttribute.EditorTypeName)
            ? CreateDefaultEditor(propertyDescriptor.PropertyType)
            : CreateEditor(Type.GetType(editorAttribute.EditorTypeName));

        return editor;
    }

    public virtual PropertyEditorBase CreateDefaultEditor(Type type) =>
        EditorResolver.TypeEditorsDictionary.TryGetValue(type, out var editorType)
            ? editorType 
            : type.IsSubclassOf(typeof(Enum))
                ? new EnumPropertyEditor()
                : new ReadOnlyTextPropertyEditor();

    public virtual PropertyEditorBase CreateEditor(Type type) => Activator.CreateInstance(type) as PropertyEditorBase ?? new ReadOnlyTextPropertyEditor();
}
