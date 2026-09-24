import { useState } from 'react';
import CreditForm from '../components/CreditForm';
import AmortizationTable from '../components/AmortizationTable';
import ExportButtons from '../components/ExportButtons';
import { createSimulation } from '../services/simulationService';

const money = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'USD' });

export default function SimulatorPage() { const [simulation, setSimulation] = useState(null); const [error, setError] = useState(''); const [loading, setLoading] = useState(false); const submit = async (data) => { setLoading(true); setError(''); try { setSimulation(await createSimulation(data)); } catch (e) { setError(e.response?.data?.message || 'No se pudo crear la simulación.'); } finally { setLoading(false); } }; return <><div className="page-heading"><div><p className="eyebrow">Centro de decisión</p><h1>Simula tu crédito</h1><p>Explora el costo real de cada alternativa antes de elegir.</p></div></div><div className="workspace-grid"><section className="panel"><h2>Datos del crédito</h2><CreditForm onSubmit={submit} loading={loading} />{error && <p className="message error">{error}</p>}</section>{simulation && <section className="panel result-panel"><div className="result-header"><div><p className="eyebrow">Resultado</p><h2>{simulation.creditTypeName}</h2></div><ExportButtons simulationId={simulation.id} /></div><div className="metrics"><div><span>Cuota inicial</span><strong>{money.format(simulation.installments?.[0]?.payment || 0)}</strong></div><div><span>Interés total</span><strong>{money.format(simulation.totalInterest || 0)}</strong></div><div><span>Total pagado</span><strong>{money.format(simulation.totalPaid || 0)}</strong></div></div><AmortizationTable simulation={simulation} /></section>}</div></>; }
