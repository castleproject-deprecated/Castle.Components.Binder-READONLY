// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.Generator.Components.Database
{
	using System;

	[Serializable]
	public abstract class ActiveRecordPropertyDescriptor
	{
		private bool _generate = true;
		private String _columnName;
		private String _columnTypeName = "VARCHAR";
		private String _propertyName;
		protected Type _propertyType;
		private bool _insert = true;
		private bool _update = true;

		public ActiveRecordPropertyDescriptor(
			String columnName, String columnTypeName, 
			String propertyName, Type propertyType) : this(columnName, columnTypeName,propertyName)
		{
			if (propertyType == null) throw new ArgumentNullException("propertyType can't be null");

			_propertyType = propertyType;
		}

		public ActiveRecordPropertyDescriptor(String columnName, 
			String columnTypeName, String propertyName)
		{
			if (columnName == null) throw new ArgumentNullException("columnName can't be null");
			if (columnTypeName == null) throw new ArgumentNullException("columnTypeName can't be null");
			if (propertyName == null) throw new ArgumentNullException("propertyName can't be null");

			_columnName = columnName;
			_columnTypeName = columnTypeName;
			_propertyName = propertyName;
		}

		public String ColumnName
		{
			get { return _columnName; }
		}

		public String ColumnTypeName
		{
			get { return _columnTypeName; }
		}

		public String PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}

		public Type PropertyType
		{
			get { return _propertyType; }
		}

		public bool Generate
		{
			get { return _generate; }
			set { _generate = value; }
		}

		public bool Insert
		{
			get { return _insert; }
			set { _insert = value; }
		}

		public bool Update
		{
			get { return _update; }
			set { _update = value; }
		}

		public override String ToString()
		{
			return _columnName;
		}

		public override bool Equals(object obj)
		{
			ActiveRecordPropertyDescriptor other = obj as ActiveRecordPropertyDescriptor;
			
			if (other == null) return false;
			
			return _columnName.Equals(other._columnName);
		}

		public override int GetHashCode()
		{
			return _columnName.GetHashCode();
		}
	}

	[Serializable]
	public class ActiveRecordPrimaryKeyDescriptor : ActiveRecordFieldDescriptor
	{
		private String _generatorType;

		public ActiveRecordPrimaryKeyDescriptor(
			String columnName, String columnTypeName, 
			String propertyName, Type propertyType, String generatorType) : 
			base(columnName, columnTypeName, propertyName, propertyType, false)
		{
			_generatorType = generatorType;
		}
	}

	[Serializable]
	public class ActiveRecordFieldDescriptor : ActiveRecordPropertyDescriptor
	{
		private bool _nullable = false;

		public ActiveRecordFieldDescriptor(
			String columnName, String columnTypeName, 
			String propertyName, Type propertyType, bool _nullable) : 
			base(columnName, columnTypeName, propertyName, propertyType)
		{
			this._nullable = _nullable;
		}
	}

	[Serializable]
	public abstract class ActiveRecordPropertyRelationDescriptor : ActiveRecordPropertyDescriptor
	{
		private ActiveRecordDescriptor _targetType;
		private String _relationType;
		private String _where;
		private String _orderBy;
		private String _cascade;
		private String _outerJoin;
		private bool _lazy;
		private bool _proxy;
		private bool _inverse;

		public ActiveRecordPropertyRelationDescriptor(String columnName, String columnTypeName, 
			String propertyName, String relationType, ActiveRecordDescriptor targetType) : 
			base(columnName, columnTypeName, propertyName)
		{
			_relationType = relationType;
			_targetType = targetType;
		}

		public String RelationType
		{
			get { return _relationType; }
		}

		public ActiveRecordDescriptor TargetType
		{
			get { return _targetType; }
		}

		public String Where
		{
			get { return _where; }
			set { _where = value; }
		}

		public String OrderBy
		{
			get { return _orderBy; }
			set { _orderBy = value; }
		}

		public String Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public String OuterJoin
		{
			get { return _outerJoin; }
			set { _outerJoin = value; }
		}

		public bool Proxy
		{
			get { return _proxy; }
			set { _proxy = value; }
		}

		public bool Lazy
		{
			get { return _lazy; }
			set { _lazy = value; }
		}

		public bool Inverse
		{
			get { return _inverse; }
			set { _inverse = value; }
		}
	}

	[Serializable]
	public class ActiveRecordBelongsToDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		public ActiveRecordBelongsToDescriptor(String _columnName, 
			String _propertyName, ActiveRecordDescriptor _targetType) : 
			base(_columnName, "", _propertyName, "BelongsTo", _targetType)
		{
		}
	}

	[Serializable]
	public class ActiveRecordHasManyDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		public ActiveRecordHasManyDescriptor(String columnName,
			String propertyName, Type propertyType, ActiveRecordDescriptor targetType) : 
			base(columnName, String.Empty, propertyName, "HasMany", targetType)
		{
			_propertyType = propertyType;
		}
	}

	[Serializable]
	public class ActiveRecordHasOneDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		public ActiveRecordHasOneDescriptor(String _columnName, 
			String _propertyName, ActiveRecordDescriptor _targetType) : 
			base(_columnName, "", _propertyName, "HasOne", _targetType)
		{
		}
	}

	[Serializable]
	public class ActiveRecordHasAndBelongsToManyDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		private String _columnKey;

		public ActiveRecordHasAndBelongsToManyDescriptor(String _columnName, String _columnTypeName, 
			String _propertyName, ActiveRecordDescriptor _targetType, String _columnKey) : 
			base(_columnName, _columnTypeName, _propertyName, "HasAndBelongsToMany", _targetType)
		{
			this._columnKey = _columnKey;
		}
	}
}
