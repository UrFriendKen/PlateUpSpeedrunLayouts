﻿using System;
using System.Reflection;
using UnityEngine;

namespace KitchenSpeedrunLayouts.Utils
{
    internal static class GameObjectUtils
    {
        /// <summary>
        /// Given a GameObject and a path, such as "A" or "A/B/C", find a child object.
        /// </summary>
        /// <param name="prefab">The parent object.</param>
        /// <param name="childPath">The path to search for the child at.</param>
        /// <returns>The child GameObject, if found. Otherwise null.</returns>
        public static GameObject GetChildObject(GameObject prefab, string childPath)
        {
            Transform transform = prefab.transform;
            string[] array = childPath.Split('/');
            string[] array2 = array;
            foreach (string n in array2)
            {
                transform = transform?.Find(n);
            }
            if (transform != null)
            {
                return transform.gameObject;
            }
            return null;
        }

        /// <summary>
        /// Given a GameObject and a path, such as "A" or "A/B/C", find a child object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="childPath">The path to search for the child at.</param>
        /// <returns>The child GameObject, if found. Otherwise null.</returns>
        public static GameObject GetChild(this GameObject parent, string childPath)
        {
            return GetChildObject(parent, childPath);
        }

        /// <summary>
        /// Given a GameObject and a child index, find a child object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="childIndex">The index of the child.</param>
        /// <returns>The child GameObject, if found. Otherwise throws an error.</returns>
        public static GameObject GetChild(this GameObject parent, int childIndex)
        {
            return parent.transform.GetChild(childIndex).gameObject;
        }

        /// <summary>
        /// Gets the amount of children of a GameObject.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <returns>The number of children of the parent.</returns>
        public static int GetChildCount(this GameObject parent)
        {
            return parent.transform.childCount;
        }

        /// <summary>
        /// Checks if a GameObject has the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="gameObject">The GameObject to check.</param>
        /// <returns>True if gameObject has the component.</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return (UnityEngine.Object)gameObject.GetComponent<T>() != (UnityEngine.Object)null;
        }

        /// <summary>
        /// Adds a component to a GameObject if it does not already have a component of that type.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="gameObject">The GameObject to check.</param>
        /// <returns>True if the component was added.</returns>
        public static T TryAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
            {
                return gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Clones a component from one GameObject to another
        /// </summary>
        /// <param name="original">The original component to copy.</param>
        /// <param name="destination">The GameObject to assign the clone to.</param>
        /// <returns>Cloned component</returns>
        public static Component CopyComponent(Component original, GameObject destination)
        {
            Type type = original.GetType();
            Component component = destination.AddComponent(type);
            FieldInfo[] fields = type.GetFields();
            FieldInfo[] array = fields;
            foreach (FieldInfo fieldInfo in array)
            {
                fieldInfo.SetValue(component, fieldInfo.GetValue(original));
            }
            return component;
        }
    }
}
