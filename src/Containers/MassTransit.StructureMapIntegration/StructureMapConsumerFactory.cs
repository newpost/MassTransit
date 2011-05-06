// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.StructureMapIntegration
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using StructureMap;

	public class StructureMapConsumerFactory<T> :
		IConsumerFactory<T>
		where T : class
	{
		readonly IContainer _container;

		public StructureMapConsumerFactory(IContainer container)
		{
			_container = container;
		}

		public IEnumerable<Action<TMessage>> GetConsumer<TMessage>(Func<T, Action<TMessage>> callback)
		{
			using (IContainer nestedContainer = _container.GetNestedContainer())
			{
				var consumer = nestedContainer.GetInstance<T>();
				if (consumer == null)
					throw new ConfigurationException(string.Format("Unable to resolve type '{0}' from container: ", typeof (T)));

				Action<TMessage> result = callback(consumer);
				if (result == null)
					yield break;

				yield return result;
			}
		}
	}
}