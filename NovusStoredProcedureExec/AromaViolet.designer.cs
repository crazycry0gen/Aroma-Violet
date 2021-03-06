﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NovusStoredProcedureExec
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Novus1")]
	public partial class AromaVioletDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertSystemSetting(SystemSetting instance);
    partial void UpdateSystemSetting(SystemSetting instance);
    partial void DeleteSystemSetting(SystemSetting instance);
    partial void InsertSystemProcedure(SystemProcedure instance);
    partial void UpdateSystemProcedure(SystemProcedure instance);
    partial void DeleteSystemProcedure(SystemProcedure instance);
    partial void InsertSystemProcedureMessage(SystemProcedureMessage instance);
    partial void UpdateSystemProcedureMessage(SystemProcedureMessage instance);
    partial void DeleteSystemProcedureMessage(SystemProcedureMessage instance);
    partial void InsertSystemIntervalSpecifier(SystemIntervalSpecifier instance);
    partial void UpdateSystemIntervalSpecifier(SystemIntervalSpecifier instance);
    partial void DeleteSystemIntervalSpecifier(SystemIntervalSpecifier instance);
    #endregion
		
		public AromaVioletDataContext() : 
				base(global::NovusStoredProcedureExec.Properties.Settings.Default.Novus1ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public AromaVioletDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AromaVioletDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AromaVioletDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AromaVioletDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<SystemSetting> SystemSettings
		{
			get
			{
				return this.GetTable<SystemSetting>();
			}
		}
		
		public System.Data.Linq.Table<SystemProcedure> SystemProcedures
		{
			get
			{
				return this.GetTable<SystemProcedure>();
			}
		}
		
		public System.Data.Linq.Table<SystemProcedureMessage> SystemProcedureMessages
		{
			get
			{
				return this.GetTable<SystemProcedureMessage>();
			}
		}
		
		public System.Data.Linq.Table<SystemIntervalSpecifier> SystemIntervalSpecifiers
		{
			get
			{
				return this.GetTable<SystemIntervalSpecifier>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SystemSetting")]
	public partial class SystemSetting : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _SettingId;
		
		private string _SettingKey;
		
		private string _SettingValue;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSettingIdChanging(int value);
    partial void OnSettingIdChanged();
    partial void OnSettingKeyChanging(string value);
    partial void OnSettingKeyChanged();
    partial void OnSettingValueChanging(string value);
    partial void OnSettingValueChanged();
    #endregion
		
		public SystemSetting()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SettingId", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int SettingId
		{
			get
			{
				return this._SettingId;
			}
			set
			{
				if ((this._SettingId != value))
				{
					this.OnSettingIdChanging(value);
					this.SendPropertyChanging();
					this._SettingId = value;
					this.SendPropertyChanged("SettingId");
					this.OnSettingIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SettingKey", DbType="NVarChar(MAX)")]
		public string SettingKey
		{
			get
			{
				return this._SettingKey;
			}
			set
			{
				if ((this._SettingKey != value))
				{
					this.OnSettingKeyChanging(value);
					this.SendPropertyChanging();
					this._SettingKey = value;
					this.SendPropertyChanged("SettingKey");
					this.OnSettingKeyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SettingValue", DbType="NVarChar(MAX)")]
		public string SettingValue
		{
			get
			{
				return this._SettingValue;
			}
			set
			{
				if ((this._SettingValue != value))
				{
					this.OnSettingValueChanging(value);
					this.SendPropertyChanging();
					this._SettingValue = value;
					this.SendPropertyChanged("SettingValue");
					this.OnSettingValueChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SystemProcedure")]
	public partial class SystemProcedure : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _SystemProcedureId;
		
		private string _ProcedureName;
		
		private string _Proceduredescription;
		
		private int _IntervalSpecifierId;
		
		private int _Interval;
		
		private bool _Active;
		
		private System.DateTime _LastRun;
		
		private EntitySet<SystemProcedureMessage> _SystemProcedureMessages;
		
		private EntityRef<SystemIntervalSpecifier> _SystemIntervalSpecifier;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSystemProcedureIdChanging(System.Guid value);
    partial void OnSystemProcedureIdChanged();
    partial void OnProcedureNameChanging(string value);
    partial void OnProcedureNameChanged();
    partial void OnProceduredescriptionChanging(string value);
    partial void OnProceduredescriptionChanged();
    partial void OnIntervalSpecifierIdChanging(int value);
    partial void OnIntervalSpecifierIdChanged();
    partial void OnIntervalChanging(int value);
    partial void OnIntervalChanged();
    partial void OnActiveChanging(bool value);
    partial void OnActiveChanged();
    partial void OnLastRunChanging(System.DateTime value);
    partial void OnLastRunChanged();
    #endregion
		
		public SystemProcedure()
		{
			this._SystemProcedureMessages = new EntitySet<SystemProcedureMessage>(new Action<SystemProcedureMessage>(this.attach_SystemProcedureMessages), new Action<SystemProcedureMessage>(this.detach_SystemProcedureMessages));
			this._SystemIntervalSpecifier = default(EntityRef<SystemIntervalSpecifier>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SystemProcedureId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid SystemProcedureId
		{
			get
			{
				return this._SystemProcedureId;
			}
			set
			{
				if ((this._SystemProcedureId != value))
				{
					this.OnSystemProcedureIdChanging(value);
					this.SendPropertyChanging();
					this._SystemProcedureId = value;
					this.SendPropertyChanged("SystemProcedureId");
					this.OnSystemProcedureIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProcedureName", DbType="NVarChar(MAX)")]
		public string ProcedureName
		{
			get
			{
				return this._ProcedureName;
			}
			set
			{
				if ((this._ProcedureName != value))
				{
					this.OnProcedureNameChanging(value);
					this.SendPropertyChanging();
					this._ProcedureName = value;
					this.SendPropertyChanged("ProcedureName");
					this.OnProcedureNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Proceduredescription", DbType="NVarChar(MAX)")]
		public string Proceduredescription
		{
			get
			{
				return this._Proceduredescription;
			}
			set
			{
				if ((this._Proceduredescription != value))
				{
					this.OnProceduredescriptionChanging(value);
					this.SendPropertyChanging();
					this._Proceduredescription = value;
					this.SendPropertyChanged("Proceduredescription");
					this.OnProceduredescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IntervalSpecifierId", DbType="Int NOT NULL")]
		public int IntervalSpecifierId
		{
			get
			{
				return this._IntervalSpecifierId;
			}
			set
			{
				if ((this._IntervalSpecifierId != value))
				{
					if (this._SystemIntervalSpecifier.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnIntervalSpecifierIdChanging(value);
					this.SendPropertyChanging();
					this._IntervalSpecifierId = value;
					this.SendPropertyChanged("IntervalSpecifierId");
					this.OnIntervalSpecifierIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Interval", DbType="Int NOT NULL")]
		public int Interval
		{
			get
			{
				return this._Interval;
			}
			set
			{
				if ((this._Interval != value))
				{
					this.OnIntervalChanging(value);
					this.SendPropertyChanging();
					this._Interval = value;
					this.SendPropertyChanged("Interval");
					this.OnIntervalChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Active", DbType="Bit NOT NULL")]
		public bool Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				if ((this._Active != value))
				{
					this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastRun", DbType="DateTime NOT NULL")]
		public System.DateTime LastRun
		{
			get
			{
				return this._LastRun;
			}
			set
			{
				if ((this._LastRun != value))
				{
					this.OnLastRunChanging(value);
					this.SendPropertyChanging();
					this._LastRun = value;
					this.SendPropertyChanged("LastRun");
					this.OnLastRunChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SystemProcedure_SystemProcedureMessage", Storage="_SystemProcedureMessages", ThisKey="SystemProcedureId", OtherKey="SystemProcedureId")]
		public EntitySet<SystemProcedureMessage> SystemProcedureMessages
		{
			get
			{
				return this._SystemProcedureMessages;
			}
			set
			{
				this._SystemProcedureMessages.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SystemIntervalSpecifier_SystemProcedure", Storage="_SystemIntervalSpecifier", ThisKey="IntervalSpecifierId", OtherKey="IntervalSpecifierId", IsForeignKey=true)]
		public SystemIntervalSpecifier SystemIntervalSpecifier
		{
			get
			{
				return this._SystemIntervalSpecifier.Entity;
			}
			set
			{
				SystemIntervalSpecifier previousValue = this._SystemIntervalSpecifier.Entity;
				if (((previousValue != value) 
							|| (this._SystemIntervalSpecifier.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._SystemIntervalSpecifier.Entity = null;
						previousValue.SystemProcedures.Remove(this);
					}
					this._SystemIntervalSpecifier.Entity = value;
					if ((value != null))
					{
						value.SystemProcedures.Add(this);
						this._IntervalSpecifierId = value.IntervalSpecifierId;
					}
					else
					{
						this._IntervalSpecifierId = default(int);
					}
					this.SendPropertyChanged("SystemIntervalSpecifier");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_SystemProcedureMessages(SystemProcedureMessage entity)
		{
			this.SendPropertyChanging();
			entity.SystemProcedure = this;
		}
		
		private void detach_SystemProcedureMessages(SystemProcedureMessage entity)
		{
			this.SendPropertyChanging();
			entity.SystemProcedure = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SystemProcedureMessage")]
	public partial class SystemProcedureMessage : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _SystemProcedureMessageId;
		
		private System.Guid _SystemProcedureId;
		
		private System.DateTime _MessageDate;
		
		private string _Message;
		
		private EntityRef<SystemProcedure> _SystemProcedure;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSystemProcedureMessageIdChanging(System.Guid value);
    partial void OnSystemProcedureMessageIdChanged();
    partial void OnSystemProcedureIdChanging(System.Guid value);
    partial void OnSystemProcedureIdChanged();
    partial void OnMessageDateChanging(System.DateTime value);
    partial void OnMessageDateChanged();
    partial void OnMessageChanging(string value);
    partial void OnMessageChanged();
    #endregion
		
		public SystemProcedureMessage()
		{
			this._SystemProcedure = default(EntityRef<SystemProcedure>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SystemProcedureMessageId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid SystemProcedureMessageId
		{
			get
			{
				return this._SystemProcedureMessageId;
			}
			set
			{
				if ((this._SystemProcedureMessageId != value))
				{
					this.OnSystemProcedureMessageIdChanging(value);
					this.SendPropertyChanging();
					this._SystemProcedureMessageId = value;
					this.SendPropertyChanged("SystemProcedureMessageId");
					this.OnSystemProcedureMessageIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SystemProcedureId", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid SystemProcedureId
		{
			get
			{
				return this._SystemProcedureId;
			}
			set
			{
				if ((this._SystemProcedureId != value))
				{
					if (this._SystemProcedure.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSystemProcedureIdChanging(value);
					this.SendPropertyChanging();
					this._SystemProcedureId = value;
					this.SendPropertyChanged("SystemProcedureId");
					this.OnSystemProcedureIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MessageDate", DbType="DateTime NOT NULL")]
		public System.DateTime MessageDate
		{
			get
			{
				return this._MessageDate;
			}
			set
			{
				if ((this._MessageDate != value))
				{
					this.OnMessageDateChanging(value);
					this.SendPropertyChanging();
					this._MessageDate = value;
					this.SendPropertyChanged("MessageDate");
					this.OnMessageDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Message", DbType="NVarChar(MAX)")]
		public string Message
		{
			get
			{
				return this._Message;
			}
			set
			{
				if ((this._Message != value))
				{
					this.OnMessageChanging(value);
					this.SendPropertyChanging();
					this._Message = value;
					this.SendPropertyChanged("Message");
					this.OnMessageChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SystemProcedure_SystemProcedureMessage", Storage="_SystemProcedure", ThisKey="SystemProcedureId", OtherKey="SystemProcedureId", IsForeignKey=true)]
		public SystemProcedure SystemProcedure
		{
			get
			{
				return this._SystemProcedure.Entity;
			}
			set
			{
				SystemProcedure previousValue = this._SystemProcedure.Entity;
				if (((previousValue != value) 
							|| (this._SystemProcedure.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._SystemProcedure.Entity = null;
						previousValue.SystemProcedureMessages.Remove(this);
					}
					this._SystemProcedure.Entity = value;
					if ((value != null))
					{
						value.SystemProcedureMessages.Add(this);
						this._SystemProcedureId = value.SystemProcedureId;
					}
					else
					{
						this._SystemProcedureId = default(System.Guid);
					}
					this.SendPropertyChanged("SystemProcedure");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SystemIntervalSpecifier")]
	public partial class SystemIntervalSpecifier : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _IntervalSpecifierId;
		
		private string _IntervalSpecifierName;
		
		private int _MilisecondConverter;
		
		private EntitySet<SystemProcedure> _SystemProcedures;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIntervalSpecifierIdChanging(int value);
    partial void OnIntervalSpecifierIdChanged();
    partial void OnIntervalSpecifierNameChanging(string value);
    partial void OnIntervalSpecifierNameChanged();
    partial void OnMilisecondConverterChanging(int value);
    partial void OnMilisecondConverterChanged();
    #endregion
		
		public SystemIntervalSpecifier()
		{
			this._SystemProcedures = new EntitySet<SystemProcedure>(new Action<SystemProcedure>(this.attach_SystemProcedures), new Action<SystemProcedure>(this.detach_SystemProcedures));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IntervalSpecifierId", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int IntervalSpecifierId
		{
			get
			{
				return this._IntervalSpecifierId;
			}
			set
			{
				if ((this._IntervalSpecifierId != value))
				{
					this.OnIntervalSpecifierIdChanging(value);
					this.SendPropertyChanging();
					this._IntervalSpecifierId = value;
					this.SendPropertyChanged("IntervalSpecifierId");
					this.OnIntervalSpecifierIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IntervalSpecifierName", DbType="NVarChar(MAX)")]
		public string IntervalSpecifierName
		{
			get
			{
				return this._IntervalSpecifierName;
			}
			set
			{
				if ((this._IntervalSpecifierName != value))
				{
					this.OnIntervalSpecifierNameChanging(value);
					this.SendPropertyChanging();
					this._IntervalSpecifierName = value;
					this.SendPropertyChanged("IntervalSpecifierName");
					this.OnIntervalSpecifierNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MilisecondConverter", DbType="Int NOT NULL")]
		public int MilisecondConverter
		{
			get
			{
				return this._MilisecondConverter;
			}
			set
			{
				if ((this._MilisecondConverter != value))
				{
					this.OnMilisecondConverterChanging(value);
					this.SendPropertyChanging();
					this._MilisecondConverter = value;
					this.SendPropertyChanged("MilisecondConverter");
					this.OnMilisecondConverterChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SystemIntervalSpecifier_SystemProcedure", Storage="_SystemProcedures", ThisKey="IntervalSpecifierId", OtherKey="IntervalSpecifierId")]
		public EntitySet<SystemProcedure> SystemProcedures
		{
			get
			{
				return this._SystemProcedures;
			}
			set
			{
				this._SystemProcedures.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_SystemProcedures(SystemProcedure entity)
		{
			this.SendPropertyChanging();
			entity.SystemIntervalSpecifier = this;
		}
		
		private void detach_SystemProcedures(SystemProcedure entity)
		{
			this.SendPropertyChanging();
			entity.SystemIntervalSpecifier = null;
		}
	}
}
#pragma warning restore 1591
