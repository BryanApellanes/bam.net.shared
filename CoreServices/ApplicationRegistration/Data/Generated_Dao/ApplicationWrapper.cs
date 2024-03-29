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
	public class ApplicationWrapper: Bam.Net.CoreServices.ApplicationRegistration.Data.Application, IHasUpdatedXrefCollectionProperties
	{
		public ApplicationWrapper()
		{
			this.UpdatedXrefCollectionProperties = new Dictionary<string, PropertyInfo>();
		}

		public ApplicationWrapper(DaoRepository repository) : this()
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

        System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.ApiHmacKey> _apiKeys;
		public override System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.ApiHmacKey> ApiKeys
		{
			get
			{
				if (_apiKeys == null)
				{
					_apiKeys = DaoRepository.ForeignKeyCollectionLoader<Bam.Net.CoreServices.ApplicationRegistration.Data.Application, Bam.Net.CoreServices.ApplicationRegistration.Data.ApiHmacKey>(this).ToList();
				}
				return _apiKeys;
			}
			set
			{
				_apiKeys = value;
			}
		}        System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor> _instances;
		public override System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor> Instances
		{
			get
			{
				if (_instances == null)
				{
					_instances = DaoRepository.ForeignKeyCollectionLoader<Bam.Net.CoreServices.ApplicationRegistration.Data.Application, Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor>(this).ToList();
				}
				return _instances;
			}
			set
			{
				_instances = value;
			}
		}        System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.Configuration> _configurations;
		public override System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.Configuration> Configurations
		{
			get
			{
				if (_configurations == null)
				{
					_configurations = DaoRepository.ForeignKeyCollectionLoader<Bam.Net.CoreServices.ApplicationRegistration.Data.Application, Bam.Net.CoreServices.ApplicationRegistration.Data.Configuration>(this).ToList();
				}
				return _configurations;
			}
			set
			{
				_configurations = value;
			}
		}        System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.Client> _clients;
		public override System.Collections.Generic.List<Bam.Net.CoreServices.ApplicationRegistration.Data.Client> Clients
		{
			get
			{
				if (_clients == null)
				{
					_clients = DaoRepository.ForeignKeyCollectionLoader<Bam.Net.CoreServices.ApplicationRegistration.Data.Application, Bam.Net.CoreServices.ApplicationRegistration.Data.Client>(this).ToList();
				}
				return _clients;
			}
			set
			{
				_clients = value;
			}
		}
        Bam.Net.CoreServices.ApplicationRegistration.Data.Organization _organization;
		public override Bam.Net.CoreServices.ApplicationRegistration.Data.Organization Organization
		{
			get
			{
				if (_organization == null)
				{
					_organization = (Bam.Net.CoreServices.ApplicationRegistration.Data.Organization)DaoRepository.GetParentPropertyOfChild(this, typeof(Bam.Net.CoreServices.ApplicationRegistration.Data.Organization));
				}
				return _organization;
			}
			set
			{
				_organization = value;
			}
		}
        // left xref

// Left Xref property: Left -> Application ; Right -> HostDomain
		List<Bam.Net.CoreServices.ApplicationRegistration.Data.HostDomain> _hostDomains;
		public override List<Bam.Net.CoreServices.ApplicationRegistration.Data.HostDomain> HostDomains
		{
			get
			{
				if(_hostDomains == null || _hostDomains.Count == 0)
				{
					var xref = new XrefDaoCollection<Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.ApplicationHostDomain, Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.HostDomain>(DaoRepository.GetDaoInstance(this), false);
					xref.Load(DaoRepository.Database);
					_hostDomains = ((IEnumerable)xref).CopyAs<Bam.Net.CoreServices.ApplicationRegistration.Data.HostDomain>().ToList();
					SetUpdatedXrefCollectionProperty("HostDomains", this.GetType().GetProperty("HostDomains"));					
				}

				return _hostDomains;
			}
			set
			{
				_hostDomains = value;
				SetUpdatedXrefCollectionProperty("HostDomains", this.GetType().GetProperty("HostDomains"));
			}
		}
        // left xref

// Left Xref property: Left -> Application ; Right -> Machine
		List<Bam.Net.CoreServices.ApplicationRegistration.Data.Machine> _machines;
		public override List<Bam.Net.CoreServices.ApplicationRegistration.Data.Machine> Machines
		{
			get
			{
				if(_machines == null || _machines.Count == 0)
				{
					var xref = new XrefDaoCollection<Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.ApplicationMachine, Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.Machine>(DaoRepository.GetDaoInstance(this), false);
					xref.Load(DaoRepository.Database);
					_machines = ((IEnumerable)xref).CopyAs<Bam.Net.CoreServices.ApplicationRegistration.Data.Machine>().ToList();
					SetUpdatedXrefCollectionProperty("Machines", this.GetType().GetProperty("Machines"));					
				}

				return _machines;
			}
			set
			{
				_machines = value;
				SetUpdatedXrefCollectionProperty("Machines", this.GetType().GetProperty("Machines"));
			}
		}


	}
	// -- generated
}																								
