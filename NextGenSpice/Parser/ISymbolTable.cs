﻿using System;
using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Parser
{
    /// <summary>
    ///     Defines methods for manipulating existing symbols during SPICE code parsing.
    /// </summary>
    public interface ISymbolTable
    {
        /// <summary>
        ///     Locks all contents of symbol tables as defaultly visible in all scopes and starts global scope.
        /// </summary>
        void FreezeDefaults();

        /// <summary>
        ///     Gets the model parameters of given type associated with given name.
        /// </summary>
        /// <param name="modelType">Type of the model parameters.</param>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        /// <returns>True if given model was found, false otherwise.</returns>
        bool TryGetModel(Type modelType, string name, out object model);

        /// <summary>
        ///     Gets the model parameters of given type associated with given name.
        /// </summary>
        /// <typeparam name="T">Type of the model parameters.</typeparam>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        /// <returns>True if given model was found, false otherwise.</returns>
        bool TryGetModel<T>(string name, out T model);

        /// <summary>
        ///     Gets model of given type associated with given name.
        /// </summary>
        /// <param name="modelType">Type of the model parameters.</param>
        /// <param name="name">Name of the model.</param>
        /// <returns>The model.</returns>
        object GetModel(Type modelType, string name);

        /// <summary>
        ///     Gets model of given type associated with given name.
        /// </summary>
        /// <typeparam name="T">Type of the model parameters.</typeparam>
        /// <param name="name">Name of the model.</param>
        /// <returns>The model.</returns>
        T GetModel<T>(string name);

        /// <summary>
        ///     Adds model of given type and name to the symbol tables.
        /// </summary>
        /// <param name="modelType">Type of the model parameters.</param>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        void AddModel(Type modelType, object model, string name);

        /// <summary>
        ///     Adds model of given type and name to the symbol tables.
        /// </summary>
        /// <typeparam name="T">Type of the model type.</typeparam>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        void AddModel<T>(T model, string name);

        /// <summary>
        ///     Gets the subcircuit associated with given name.
        /// </summary>
        /// <param name="name">Name of the subcircuit.</param>
        /// <param name="subcircuit">Out variable to store found model in.</param>
        /// <returns></returns>
        bool TryGetSubcircuit(string name, out SubcircuitElement subcircuit);

        /// <summary>
        ///     Returns the subcircuit instance with corresponding name.
        /// </summary>
        /// <param name="name">The subcircuit name</param>
        /// <returns></returns>
        SubcircuitElement GetSubcircuit(string name);

        /// <summary>
        ///     Adds given symbol to the set of element names, returns true if it not already used.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool TryDefineElement(string name);

        /// <summary>
        ///     Tries to get node index corresponding to given node name. Returns true on success.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool TryGetNodeIndex(string name, out int index);

        /// <summary>
        ///     Gives set of node names for given set of indexes
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        IEnumerable<string> GetNodeNames(IEnumerable<int> indexes);

        /// <summary>
        ///     Adds subcircuit under given name to the symbol tables.
        /// </summary>
        /// <param name="name">Name of the subcircuit.</param>
        /// <param name="subcircuit">The subcircuit deinition.</param>
        void AddSubcircuit(string name, SubcircuitElement subcircuit);
    }
}