using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Linq;
using Bam.Net;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Newtonsoft.Json;
using Bam.Net.CoreServices.ApplicationRegistration.Data;
using Bam.Net.CoreServices.ApplicationRegistration.Data.Dao;

namespace Bam.Net.CoreServices.ApplicationRegistration.Data.Wrappers
{
	// generated
	[Serializable]
	public class ProcessDescriptorWrapper: Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor, IHasUpdatedXrefCollectionProperties
	{
		public ProcessDescriptorWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public ProcessDescriptorWrapper(DaoRepository repository) : this()
		{
			this.DaoRepository = repository;
		}

		[JsonIgnore]
		public DaoRepository DaoRepository { get; set; }

		[JsonIgnore]
		public Dictionary<string, PropertyInfo> UpdatedXrefCollectionProperties { get; set; }

		protected void SetUpdatedXrefCollectionProperty(string propertyName, PropertyInfo correspondingProperty)
		{
			if(UpdatedXrefCollectionProperties != null && !UpdatedXrefCollectionProperties.ContainsKey(propertyName))
			{
				UpdatedXrefCollectionProperties.Add(propertyName, correspondingProperty);				
			}
			else if(UpdatedXrefCollectionProperties != null)
			{
				UpdatedXrefCollectionProperties[propertyName] = correspondingProperty;				
			}
		}


        Bam.Net.CoreServices.ApplicationRegistration.Data.Application _application;
		public override Bam.Net.CoreServices.ApplicationRegistration.Data.Application Application
		{
			get
			{
				if (_application == null)
				{
					_application = (Bam.Net.CoreServices.ApplicationRegistration.Data.Application)DaoRepository.GetParentPropertyOfChild(this, typeof(Bam.Net.CoreServices.ApplicationRegistration.Data.Application));
				}
				return _application;
			}
			set
			{
				_application = value;
			}
		}        Bam.Net.CoreServices.ApplicationRegistration.Data.Machine _machine;
		public override Bam.Net.CoreServices.ApplicationRegistration.Data.Machine Machine
		{
			get
			{
				if (_machine == null)
				{
					_machine = (Bam.Net.CoreServices.ApplicationRegistration.Data.Machine)DaoRepository.GetParentPropertyOfChild(this, typeof(Bam.Net.CoreServices.ApplicationRegistration.Data.Machine));
				}
				return _machine;
			}
			set
			{
				_machine = value;
			}
		}


	}
	// -- generated
}																								
