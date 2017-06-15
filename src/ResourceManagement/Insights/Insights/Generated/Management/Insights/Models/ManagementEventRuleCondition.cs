// 
// Copyright (c) Microsoft and contributors.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the License for the specific language governing permissions and
// limitations under the License.
// 

// Warning: This code was generated by a tool.
// 
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using System;
using System.Linq;
using Microsoft.Azure.Management.Insights.Models;

namespace Microsoft.Azure.Management.Insights.Models
{
    /// <summary>
    /// A management event rule condition.
    /// </summary>
    public partial class ManagementEventRuleCondition : RuleCondition
    {
        private ManagementEventAggregationCondition _aggregation;
        
        /// <summary>
        /// Optional. Gets or sets the aggregation condition.
        /// </summary>
        public ManagementEventAggregationCondition Aggregation
        {
            get { return this._aggregation; }
            set { this._aggregation = value; }
        }
        
        private RuleDataSource _dataSource;
        
        /// <summary>
        /// Optional. Gets or sets the condition data source.
        /// </summary>
        public RuleDataSource DataSource
        {
            get { return this._dataSource; }
            set { this._dataSource = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the ManagementEventRuleCondition
        /// class.
        /// </summary>
        public ManagementEventRuleCondition()
        {
        }
    }
}