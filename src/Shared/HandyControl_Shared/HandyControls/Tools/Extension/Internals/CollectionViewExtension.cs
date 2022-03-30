using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;

namespace HandyControl.Tools.Extension
{
    public class CollectionViewExtension<TSource>
    {
        private readonly ICollectionView _view;
        private Predicate<object> _filter;
        private readonly List<SortDescription> _sortDescriptions = new List<SortDescription>();
        private readonly List<GroupDescription> _groupDescriptions = new List<GroupDescription>();

        public CollectionViewExtension(ICollectionView view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            _view = view;
            _filter = view.Filter;
            _sortDescriptions = view.SortDescriptions.ToList();
            _groupDescriptions = view.GroupDescriptions.ToList();
        }

        public void Apply()
        {
            using (_view.DeferRefresh())
            {
                _view.Filter = _filter;
                _view.SortDescriptions.Clear();
                foreach (var s in _sortDescriptions)
                {
                    _view.SortDescriptions.Add(s);
                }
                _view.GroupDescriptions.Clear();
                foreach (var g in _groupDescriptions)
                {
                    _view.GroupDescriptions.Add(g);
                }
            }
        }

        public CollectionViewExtension<TSource> ClearGrouping()
        {
            _groupDescriptions.Clear();
            return this;
        }

        public CollectionViewExtension<TSource> ClearSort()
        {
            _sortDescriptions.Clear();
            return this;
        }

        public CollectionViewExtension<TSource> ClearFilter()
        {
            _filter = null;
            return this;
        }

        public CollectionViewExtension<TSource> ClearAll()
        {
            _filter = null;
            _sortDescriptions.Clear();
            _groupDescriptions.Clear();
            return this;
        }

        public CollectionViewExtension<TSource> Where(Func<TSource, bool> predicate)
        {
            _filter = o => predicate((TSource) o);
            return this;
        }

        public CollectionViewExtension<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            return OrderBy(keySelector, true, ListSortDirection.Ascending);
        }

        public CollectionViewExtension<TSource> OrderByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            return OrderBy(keySelector, true, ListSortDirection.Descending);
        }

        public CollectionViewExtension<TSource> ThenBy<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            return OrderBy(keySelector, false, ListSortDirection.Ascending);
        }

        public CollectionViewExtension<TSource> ThenByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            return OrderBy(keySelector, false, ListSortDirection.Descending);
        }

        private CollectionViewExtension<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector, bool clear, ListSortDirection direction)
        {
            string path = GetPropertyPath(keySelector.Body);
            if (clear)
                _sortDescriptions.Clear();
            _sortDescriptions.Add(new SortDescription(path, direction));
            return this;
        }

        public CollectionViewExtension<TSource> GroupBy<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            string path = GetPropertyPath(keySelector.Body);
            _groupDescriptions.Add(new PropertyGroupDescription(path));
            return this;
        }

        private static string GetPropertyPath(System.Linq.Expressions.Expression expression)
        {
            var names = new Stack<string>();
            var expr = expression;
            while (expr != null && !(expr is ParameterExpression) && !(expr is ConstantExpression))
            {
                var memberExpr = expr as MemberExpression;
                if (memberExpr == null)
                    throw new ArgumentException("The selector body must contain only property or field access expressions");
                names.Push(memberExpr.Member.Name);
                expr = memberExpr.Expression;
            }
            return String.Join(".", names.ToArray());
        }
    }
}
