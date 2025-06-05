// ----- Helper -----
const api = p => `/api/${p}`;
const post = (u,b) =>
  fetch(u,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(b)})
    .then(async r => { if(!r.ok) throw new Error(r.status); return r.json(); });
const get  = u =>
  fetch(u)
    .then(async r => { if(!r.ok) throw new Error(r.status); return r.json(); });

function addOpt(sel, v, t){ document.querySelectorAll(sel).forEach(s => s.add(new Option(t,v))); }
function log(txt){ const o=document.getElementById('output'); o.textContent=txt+'\n'+o.textContent; }
function set(id,txt){ document.getElementById(id).textContent=txt; }

// ----- Create -----
document.getElementById('school').onsubmit   = async e => {
    e.preventDefault();
    const f=e.target, dto={ name:f.name.value };
    const {id}=await post(api('schools'),dto);
    addOpt('.school-dropdown',id,f.name.value); log(`School ${id} created`); f.reset();
};

document.getElementById('classroom').onsubmit = async e => {
    e.preventDefault();
    const f=e.target, dto={ name:f.name.value, size:f.size.value, seats:+f.seats.value, cynap:f.cynap.checked };
    const r=await post(api('classrooms'),dto);
    addOpt('.classroom-dropdown',r.id,f.name.value); log(`Room ${r.id} created`); f.reset();
};

document.getElementById('student').onsubmit  = async e => { e.preventDefault();
    const f=e.target, dto={
      firstname:f.firstname.value,
      lastname:f.lastname.value,
      track:f.track.value,
      schoolClass:f.schoolclass.value,
      gender:f.gender.value,
      birthdate:f.birthdate.value
    };
    const r=await post(api('students'),dto);
    addOpt('.student-dropdown',r.id,`${dto.firstname} ${dto.lastname}`); log(`Student ${r.id} created`); f.reset();
};

// ----- Relations -----
bind('addStudentToSchool',    (s,st) => post(api(`schools/${s}/students/${st}`),{}));
bind('addClassroomToSchool',  (s,_,c)=> post(api(`schools/${s}/classrooms/${c}`),{}));
bind('addStudentToClassroom', (_,st,c)=>post(api(`classrooms/${c}/students/${st}`),{}));

function bind(formId, fn){
  document.getElementById(formId).onsubmit = async e => {
    e.preventDefault(); const s=e.target.school?.value, st=e.target.student?.value, c=e.target.classroom?.value;
    await fn(s,st,c); log('Done');
  };
}

// ----- KPIs -----
document.getElementById('schoolValues-dropdown').onchange = async function(){
  if(!this.value) return;
  const v=await get(api(`schools/${this.value}/values`));
  set('numberOfStudents',             v.numberOfStudents);
  set('numberOfMaleStudents',         v.numberOfMaleStudents);
  set('numberOfFemaleStudents',       v.numberOfFemaleStudents);
  set('averageAgeOfStudents',         v.averageAgeOfStudents.toFixed(1));
  set('numberOfClassrooms',           v.numberOfClassrooms);
  set('classroomsWithCynap',          v.classroomsWithCynap.join(', '));
  set('classroomsWithNumberOfStudents',
      Object.entries(v.classroomsWithNumberOfStudents).map(([c,n])=>`${c}: ${n}`).join(' | '));
};

// %‑Frauen
document.getElementById('percentageOfFemaleStudentsInSchoolclass').onsubmit = async e => {
  e.preventDefault();
  const s=e.target.school.value, cl=e.target.schoolclass.value;
  const p=await get(api(`schools/${s}/classes/${cl}/female-percentage`));
  set('percentageOfFemaleStudentInSchoolclass_output',p.toFixed(1)+' %');
};

// passt Raum?
document.getElementById('isClassroomBigEnough').onsubmit = async e => {
  e.preventDefault();
  const s=e.target.school.value, r=e.target.classroom.value, cl=e.target.schoolclass.value;
  const ok=await get(api(`schools/${s}/classrooms/${r}/can-fit/${cl}`));
  set('isClassroomBigEnough_output',ok?'Ja':'Nein');
};
