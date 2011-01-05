/*
 * Copyright 2010 Ian Gilham
 * 
 * Based on work by Eric Sink found here: http://www.ericsink.com/entries/multicore_map.html
 * 
 * You may use this code under the terms of any of the following licenses, your choice:
 * 
 * 1)  The GNU General Public License, Version 2
 *      http://www.opensource.org/licenses/gpl-license.php
 * 2)  The Apache License, Version 2.0
 *      http://www.opensource.org/licenses/apache2.0.php
 * 3)  The MIT License
 *      http://www.opensource.org/licenses/mit-license.php
 * 
 * This code is unsupported sample work and I have no intention of maintaining it
 * or having it hosted anywhere. I am not looking for contributors.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IanGilham.ParallelUtils
{
    /// <summary>
    /// This class contains things which help an application use
    /// multiple CPUs/processors/cores.
    /// 
    /// More specifically, this class contains several implementations
    /// of "Map".  The name Map comes from the concepts of Map and
    /// Reduce in functional programming.  I'm no expert in that
    /// area, so I'm probably misusing the term in some way.  
    /// I apologize in advance for this egregious offense.
    /// 
    /// Anyway, the basic idea is this:
    /// 
    /// Instead of running a loop over every item in a list or
    /// array, pass your list and your code fragment into map.
    /// By allowing map to handle things, you gain the benefits
    /// of using more than one CPU or processor core.  Instead
    /// of calling your code fragment sequentially once for each 
    /// item in the list, map will use multiple threads to make
    /// those calls happen in parallel.
    /// 
    /// Obviously, if you need the items in your list to be
    /// processed in order, don't use map.  The order in which
    /// your items will be handled is not defined.
    /// 
    /// Furthermore, the arguments you pass into map need to
    /// be thread-safe.  Ideally, your threads and list of
    /// arguments aren't going to be sharing any data, certainly
    /// not trying to modify any shared data.  In practice, this
    /// isn't always the easiest way to do things, so you'll need
    /// to use locks.
    /// 
    /// Map is not a panacea.  The hardest part of using map
    /// is getting your code into a state where it can be
    /// safely and usefully executed in parallel.
    /// 
    /// Note that there is some overhead in using map.  The
    /// performance improvement of map is best when you have
    /// a few items, each of which is a big job.  If you have
    /// lots of very small tasks to perform, the overhead of
    /// creating threads will be greater than the time saved
    /// by executing them in parallel.
    /// 
    /// I have tested this code only with Visual Studio 2005
    /// under Windows XP.  I'll assume that no other compiler
    /// or environment will work, but I don't actually know.
    /// 
    /// For more information, and for the sources of inspiration
    /// I used in writing this code:
    /// 
    /// http://www.ookii.org/showpost.aspx?post=8
    /// 
    /// http://www.codeguru.com/columns/experts/article.php/c4767/
    /// 
    /// http://www.devx.com/amd/Article/32301
    /// 
    /// http://www.joelonsoftware.com/items/2006/08/01.html
    /// 
    /// http://labs.google.com/papers/mapreduce.html
    /// 
    /// http://msdn.microsoft.com/msdnmag/issues/06/09/CLRInsideOut/default.aspx
    /// 
    /// http://en.wikipedia.org/wiki/MapReduce
    /// 
    /// http://www.codethinked.com/category/Functional-Programming-Series.aspx?page=2
    /// 
    /// </summary>
    public static class Extensions
    {
        private static bool useThreads = (Environment.ProcessorCount > 1);

        /// <summary>
        /// Mode is a static class variable which determines
        /// how threads are to be used.  There are three
        /// possible values:
        /// 
        /// AlwaysUseThreads -- The Map functions use threads whether
        /// multiple CPUs are present or not.
        /// 
        /// NeverUseThreads -- The Map functions do not use threads.
        /// 
        /// UseThreadsIfMultipleProcessorsPresent -- The default behaviour.
        /// Threads are used if multiple cores are available.
        /// </summary>
        public static MultiCoreMode Mode
        {
            set
            {
                if (value == MultiCoreMode.AlwaysUseThreads)
                {
                    useThreads = true;
                }
                else if (
                    (value == MultiCoreMode.UseThreadsIfMultipleProcessorsPresent)
                    && (System.Environment.ProcessorCount > 1)
                    )
                {
                    useThreads = true;
                }
                else
                {
                    useThreads = false;
                }
            }
        }

        //public delegate TResult Operation<TSource, TResult>(TSource source);

        /// <summary>
        /// Extension method to apply a simple map function to an IEnumerable using the Task Parallel Library.
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the source IEnumerable</typeparam>
        /// <typeparam name="TResult">The type of the items in the result IEnumerable</typeparam>
        /// <param name="source">The source IEnumerable</param>
        /// <param name="function">The function to apply to the source items, 
        /// that returns an appropriate result item</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> function)
        {
            var result = new List<TResult>();
            if (useThreads)
            {
                Parallel.ForEach(source, new Action<TSource>((item) =>
                    {
                        result.Add(function(item));
                    }));
            }
            else
            {
                var resultList = new List<TResult>(); 
                foreach (var item in source)
                {
                    resultList.Add(function(item));
                }
                result = resultList;
            }
            return result;
        }
    }
}
