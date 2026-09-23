import { useEffect, useState } from 'react';
import { getCreditTypes } from '../services/creditTypeService';

export default function CreditTypeSelect({ value, onChange }) {
	const [types, setTypes] = useState([]); const [error, setError] = useState('');
	useEffect(() => { getCreditTypes().then(setTypes).catch(() => setError('No se pudo cargar el catálogo.')); }, []);
	return <label>Tipo de crédito<select required value={value} onChange={(e) => onChange(Number(e.target.value), types.find((type) => type.id === Number(e.target.value)))}><option value="">Selecciona una opción</option>{types.map((type) => <option key={type.id} value={type.id}>{type.name} · {type.annualRate}% anual</option>)}</select>{error && <small className="field-error">{error}</small>}</label>;
}
