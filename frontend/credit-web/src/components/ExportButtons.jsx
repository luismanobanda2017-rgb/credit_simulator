import { useState } from 'react';
import { downloadSimulation } from '../services/simulationService';

export default function ExportButtons({ simulationId }) {
	const [loading, setLoading] = useState(''); const [error, setError] = useState('');
	const download = async (format) => { setLoading(format); setError(''); try { await downloadSimulation(simulationId, format); } catch { setError('No se pudo descargar el archivo.'); } finally { setLoading(''); } };
	return <div className="export-actions"><span>Descargar:</span>{['pdf', 'xlsx', 'csv'].map((format) => <button key={format} className="button secondary" disabled={Boolean(loading)} onClick={() => download(format)}>{loading === format ? '...' : format.toUpperCase()}</button>)}{error && <small className="field-error">{error}</small>}</div>;
}
