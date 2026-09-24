import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import AmortizationTable from '../components/AmortizationTable';
import ExportButtons from '../components/ExportButtons';
import { getSimulation } from '../services/simulationService';

const money = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'USD' });

export default function SimulationDetailPage() { const { id } = useParams(); const [simulation, setSimulation] = useState(null); const [error, setError] = useState(''); useEffect(() => { getSimulation(id).then(setSimulation).catch(() => setError('No se encontró la simulación.')); }, [id]); if (error) return <div className="empty-state"><p className="message error">{error}</p><Link to="/history">Volver al historial</Link></div>; if (!simulation) return <p className="message">Cargando detalle...</p>; return <><div className="page-heading"><div><Link className="back-link" to="/history">← Historial</Link><p className="eyebrow">Detalle de simulación</p><h1>{simulation.creditTypeName}</h1><p>{money.format(simulation.amount)} · {simulation.months} meses · {simulation.annualRate}% anual</p></div><ExportButtons simulationId={simulation.id} /></div><section className="panel"><div className="metrics"><div><span>Método</span><strong>{simulation.method === 'French' ? 'Francés' : 'Alemán'}</strong></div><div><span>Interés total</span><strong>{money.format(simulation.totalInterest)}</strong></div><div><span>Total pagado</span><strong>{money.format(simulation.totalPaid)}</strong></div></div><AmortizationTable simulation={simulation} /></section></>; }
