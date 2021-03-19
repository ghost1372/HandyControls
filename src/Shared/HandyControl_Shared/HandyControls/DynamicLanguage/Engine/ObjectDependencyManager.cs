#region Copyright information
// <copyright file="ObjectDependencyManager.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Bernhard Millauer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    #endregion

    /// <summary>
    /// This class ensures, that a specific object lives as long a associated object is alive.
    /// </summary>
    public static class ObjectDependencyManager
    {
        /// <summary>
        /// This member holds the list of all <see cref="WeakReference"/>s and their appropriate objects.
        /// </summary>
        private static readonly Dictionary<object, List<WeakReference>> InternalList;

        /// <summary>
        /// Initializes static members of the <see cref="ObjectDependencyManager"/> class.
        /// Static Constructor. Creates a new instance of
        /// Dictionary(object, <see cref="WeakReference"/>) and set it to the <see cref="InternalList"/>.
        /// </summary>
        static ObjectDependencyManager()
        {
            InternalList = new Dictionary<object, List<WeakReference>>();
        }

        /// <summary>
        /// This method adds a new object dependency
        /// </summary>
        /// <param name="weakRefDp">The <see cref="WeakReference"/>, which ensures the live cycle of <paramref name="objToHold"/></param>
        /// <param name="objToHold">The object, which should stay alive as long <paramref name="weakRefDp"/> is alive</param>
        /// <returns>
        /// true, if the binding was successfully, otherwise false
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="objToHold"/> cannot be null
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="objToHold"/> cannot be type of <see cref="WeakReference"/>
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="WeakReference"/>.Target cannot be the same as <paramref name="objToHold"/>
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool AddObjectDependency(WeakReference weakRefDp, object objToHold)
        {
            // run the clean up to ensure that only objects are watched they are realy still alive
            CleanUp();

            // if the objToHold is null, we cannot handle this afterwards.
            if (objToHold == null)
            {
                throw new ArgumentNullException(nameof(objToHold), "The objToHold cannot be null");
            }

            // if the objToHold is a weakreference, we cannot handle this type afterwards.
            if (objToHold.GetType() == typeof(WeakReference))
            {
                throw new ArgumentException("objToHold cannot be type of WeakReference", nameof(objToHold));
            }

            // if the target of the weakreference is the objToHold, this would be a cycling play.
            if (weakRefDp.Target == objToHold)
            {
                throw new InvalidOperationException("The WeakReference.Target cannot be the same as objToHold");
            }

            // holds the status of registration of the object dependency
            var itemRegistered = false;

            // check if the objToHold is contained in the internalList.
            if (!InternalList.ContainsKey(objToHold))
            {
                // add the objToHold to the internal list.
                List<WeakReference> lst = new List<WeakReference> { weakRefDp };

                InternalList.Add(objToHold, lst);

                itemRegistered = true;
            }
            else
            {
                // otherweise, check if the weakRefDp exists and add it if necessary
                List<WeakReference> lst = InternalList[objToHold];
                if (!lst.Contains(weakRefDp))
                {
                    lst.Add(weakRefDp);

                    itemRegistered = true;
                }
            }

            // return the status of the registration
            return itemRegistered;
        }

        /// <summary>
        /// This method cleans up all independent (!<see cref="WeakReference"/>.IsAlive) objects.
        /// </summary>
        public static void CleanUp()
        {
            // call the overloaded method
            CleanUp(null);
        }

        /// <summary>
        /// This method cleans up all independent (!<see cref="WeakReference"/>.IsAlive) objects or a single object.
        /// </summary>
        /// <param name="objToRemove">
        /// If defined, the associated object dependency will be removed instead of a full CleanUp
        /// </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void CleanUp(object objToRemove)
        {
            // if a particular object is passed, remove it.
            if (objToRemove != null)
            {
                // if the key wasnt found, throw an exception.
                if (!InternalList.Remove(objToRemove))
                {
                    throw new Exception("Key was not found!");
                }

                // stop here
                return;
            }

            // perform an full clean up

            // this list will hold all keys they has to be removed
            List<object> keysToRemove = new List<object>();

            // step through all object dependenies
            foreach (KeyValuePair<object, List<WeakReference>> kvp in InternalList)
            {
                // step recursive through all weak references
                for (int i = kvp.Value.Count - 1; i >= 0; i--)
                {
                    // if this weak reference is no more alive, remove it
                    var targetReference = kvp.Value[i].Target;
                    if (targetReference == null)
                    {
                        kvp.Value.RemoveAt(i);
                    }
                }

                // if the list of weak references is empty, temove the whole entry
                if (kvp.Value.Count == 0)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            // step recursive through all keys that have to be remove
            for (int i = keysToRemove.Count - 1; i >= 0; i--)
            {
                // remove the key from the internalList
                InternalList.Remove(keysToRemove[i]);
            }

            // clear up the keysToRemove
            keysToRemove.Clear();
        }
    }
}